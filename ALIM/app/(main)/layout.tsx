import type React from "react";
import Header from "@/components/header";
import PulsingCircle from "@/components/pulsing-circle";
import { SessionProvider } from "next-auth/react";
import SessionGuard from "@/lib/SessionGuard";
import { auth } from "@/auth";

// This layout ONLY applies to pages inside the (main) group, like your homepage.
export default async function MainLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  const session = await auth();

  return (
    <div className="relative z-10 flex min-h-screen flex-col">
      <Header session={session} />
      <main className="flex flex-grow flex-col">{children}</main>
      <PulsingCircle />
    </div>
  );
}
