// auth.ts
import nextAuth from "next-auth";
import Keycloak from "next-auth/providers/keycloak";

async function refreshAccessToken(token: any) {
  try {
    console.log("Attempting refresh with token:", {
      hasRefreshToken: !!token.refreshToken,
      refreshToken: token.refreshToken?.substring(0, 20) + "...",
      expiresAt: new Date(token.accessTokenExpires),
    });

    const url = `${process.env.AUTH_KEYCLOAK_ISSUER}/protocol/openid-connect/token`;

    const response = await fetch(url, {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: new URLSearchParams({
        grant_type: "refresh_token",
        refresh_token: token.refreshToken,
        client_id: process.env.AUTH_KEYCLOAK_ID!,
        client_secret: process.env.AUTH_KEYCLOAK_SECRET!,
      }),
    });

    const errorData = await response.json();

    if (!response.ok) {
      console.error("Refresh failed:", {
        status: response.status,
        error: errorData.error,
        error_description: errorData.error_description,
      });

      // Force re-login on invalid_grant
      return {
        ...token,
        error: "RefreshAccessTokenError",
      };
    }

    console.log("Token refreshed successfully");
    return {
      ...token,
      accessToken: errorData.access_token,
      idToken: errorData.id_token,
      refreshToken: errorData.refresh_token ?? token.refreshToken,
      accessTokenExpires: Date.now() + errorData.expires_in * 1000,
    };
  } catch (error) {
    console.error("Refresh token error:", error);
    return {
      ...token,
      error: "RefreshAccessTokenError",
    };
  }
}

export const { handlers, auth, signIn, signOut } = nextAuth({
  providers: [
    Keycloak({
      clientId: process.env.AUTH_KEYCLOAK_ID!,
      clientSecret: process.env.AUTH_KEYCLOAK_SECRET!,
      issuer: process.env.AUTH_KEYCLOAK_ISSUER!,
      authorization: {
        params: {
          scope: "openid profile email offline_access",
        },
      },
    }),
  ],
  trustHost: true,
  basePath: "/api/auth",
  callbacks: {
    async jwt({ token, account }) {
      if (account) {
        console.log("Initial login, storing tokens:", {
          hasOfflineAccess: !!account.refresh_token,
          scope: account.scope,
        });
        return {
          id: account.providerAccountId,
          accessToken: account.access_token,
          idToken: account.id_token,
          refreshToken: account.refresh_token,
          accessTokenExpires: account.expires_at ? account.expires_at * 1000 : 0,
        };
      }

      if (token.accessTokenExpires && Date.now() < (token.accessTokenExpires as number) - 5 * 60 * 1000) {
        return token;
      }

      console.log("Token expired, attempting refresh...");
      return await refreshAccessToken(token);
    },

    async session({ session, token }) {
      if (token.error === "RefreshAccessTokenError") {
        console.log("Session has refresh error, forcing re-login");
        return {
          ...session,
          error: "RefreshAccessTokenError",
        };
      }

      return {
        ...session,
        user: {
          ...session.user,
          id: token.id,
        },
        accessToken: token.accessToken,
        refreshToken: token.refreshToken,
        idToken: token.idToken,
        accessTokenExpires: token.accessTokenExpires,
      };
    },
  },
  pages: {
    signIn: "/login",
    error: "/error",
  },
});
