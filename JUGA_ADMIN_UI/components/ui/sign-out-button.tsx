"use client";

interface SignOutProps {
  readonly name: string;
}
export const generateKeycloakLogoutUrl = (redirectUrl: string, idToken?: string): string => {
  const CLIENT_ID = process.env.NEXT_PUBLIC_AUTH_KEYCLOAK_ID ?? "";
  const AUTH_KEYCLOAK_ISSUER = process.env.NEXT_PUBLIC_AUTH_KEYCLOAK_ISSUER ?? "";
  const urlParams = new URLSearchParams();
  urlParams.append("client_id", CLIENT_ID);
  urlParams.append("post_logout_redirect_uri", `${redirectUrl}/api/auth/logout`);
  if (idToken) {
    urlParams.append("id_token_hint", idToken);
  }
  return `${AUTH_KEYCLOAK_ISSUER}/protocol/openid-connect/logout?${urlParams.toString()}`;
};

export default function SignOut() {
  return (
    <div id="gooey-btn" className="relative flex items-center group" style={{ filter: "url(#gooey-filter)" }}>
      <a className="px-6 py-2 rounded-full bg-white text-black font-normal text-xs transition-all duration-300 hover:bg-white/90 cursor-pointer h-8 flex items-center z-10" href={generateKeycloakLogoutUrl(process.env.NEXT_PUBLIC_AUTH_URL ?? "")}>
        Çıkış
      </a>
    </div>
  );
}
