import nextAuth from "next-auth";
import Keycloak from "next-auth/providers/keycloak";

export const { handlers, auth, signIn, signOut } = nextAuth({
  providers: [Keycloak],
  trustHost: true,
  callbacks: {
    async jwt({ token, account }) {
      // Add access_token from the account to the token after sign-in
      if (account?.access_token) {
        token.accessToken = account.access_token;
      }
      return token;
    },
    async session({ session, token }) {
      // Expose accessToken from the token to the session object
      session.accessToken = token.accessToken;
      return session;
    },
  },
});
