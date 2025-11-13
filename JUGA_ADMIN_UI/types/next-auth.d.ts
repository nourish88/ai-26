// types/next-auth.d.ts
import type { DefaultSession } from "next-auth";
import type { JWT } from "next-auth/jwt";

declare module "next-auth" {
  interface User {
    id?: string;
    name?: string;
    email?: string;
    accessToken?: string;
    refreshToken?: string;
    idToken?: string;
    pbik?: string;
  }

  interface Session extends DefaultSession {
    user?: {
      id?: string;
      name?: string;
      email?: string;
      projects?: string[];
      pbik?: string;
    };
    accessToken?: string;
    refreshToken?: string;
    idToken?: string;
    accessTokenExpires?: number;
    error?: string;
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    id?: string;
    accessToken?: string;
    refreshToken?: string;
    idToken?: string;
    accessTokenExpires?: number;
    user_projects?: string[];
    pbik?: string;
    error?: string;
  }
}
