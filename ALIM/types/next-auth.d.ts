import "next-auth";
import type { DefaultSession } from "next-auth";
import { JWT } from "next-auth/jwt";

declare module "next-auth" {
  interface Session extends DefaultSession {
    accessToken?: string;
    refreshToken?: string;
    idToken?: string;
    error?: string;
    pbik?: string;
  }
  interface User {
    accessToken?: string;
    idToken?: string;
    refreshToken?: string;
    pbik?: string;
  }
}

declare module "next-auth" {
  interface Session {
    user: {
      projects?: string[];
      name?: string;
      email?: string;
    };
    accessToken?: string;
    error?: string;
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    user_projects?: string[];
    accessToken?: string;
    refreshToken?: string;
    expiresAt?: number;
    error?: string;
  }
}

declare global {
  interface Token extends JWT {
    idToken?: string;
    accessToken?: string;
    refreshToken?: string;
    expiresAt?: number;
    refreshTokenExpiresAt?: number;
    error?: string;
    userName?: string;
  }
}
