import type React from "react";
import type { Metadata } from "next";
import { GeistSans } from "geist/font/sans";
import { GeistMono } from "geist/font/mono";
import { Figtree } from "next/font/google";
import { Instrument_Serif } from "next/font/google";
import { Suspense } from "react";
import { Toaster } from "@/components/ui/sonner";
import ShaderBackground from "@/components/shader-background";

import "./globals.css";
import SessionGuard from "@/lib/SessionGuard";
import { SessionProvider } from "next-auth/react";

const figtree = Figtree({
  subsets: ["latin"],
  weight: ["300", "400", "500", "600", "700"],
  variable: "--font-figtree",
  display: "swap",
});

const instrumentSerif = Instrument_Serif({
  subsets: ["latin"],
  weight: ["400"],
  style: ["normal", "italic"],
  variable: "--font-instrument-serif",
  display: "swap",
});

export const metadata: Metadata = {
  title: "ALIM",
  description: "Created with v0",
  generator: "v0.app",
};

// This is the TRUE Root Layout. It wraps EVERYTHING but has no visual structure.
export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body className={`${figtree.variable} ${GeistSans.variable} ${GeistMono.variable} ${instrumentSerif.variable} font-sans`}>
        <Suspense fallback={<div>Loading...</div>}>
          {/* The background is shared by all layouts */}
          <ShaderBackground>
            <SessionProvider>
              <SessionGuard>{children}</SessionGuard>
            </SessionProvider>
            <Toaster richColors position="top-right" />
          </ShaderBackground>
        </Suspense>
      </body>
    </html>
  );
}
