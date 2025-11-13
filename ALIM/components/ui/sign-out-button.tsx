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

export default function SignOut({ name }: SignOutProps) {
  return (
    <div id="gooey-btn" className="relative flex items-center group" style={{ filter: "url(#gooey-filter)" }}>
      <button className="absolute right-0 px-2.5 py-2 rounded-full bg-white text-black font-normal text-xs transition-all duration-300 hover:bg-white/90 cursor-pointer h-8 flex items-center justify-center -translate-x-10 group-hover:-translate-x-19 z-0">
        <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M7 17L17 7M17 7H7M17 7V17" />
        </svg>
      </button>
      <a className="px-6 py-2 rounded-full bg-white text-black font-normal text-xs transition-all duration-300 hover:bg-white/90 cursor-pointer h-8 flex items-center z-10" href={generateKeycloakLogoutUrl(process.env.NEXT_PUBLIC_AUTH_URL ?? "")}>
        Çıkış
      </a>
    </div>
  );
}
