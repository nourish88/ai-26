// components/auth/SessionGuard.tsx
"use client";

import { useSession, signOut } from "next-auth/react";
import { useEffect } from "react";
import { toast } from "sonner";

// This component wraps your main layout or pages
export function SessionGuard({ children }: { children: React.ReactNode }) {
  const { data: session, status } = useSession();

  useEffect(() => {
    // Check if the session object has the error flag we set in the auth.ts callback
    if (session?.error === "RefreshAccessTokenError") {
      toast.error("Your session has expired. Please sign in again.");
      // The signOut function will redirect the user to the login page.
      signOut();
    }
  }, [session]);

  // You can also add a loading state while the session is being checked
  if (status === "loading") {
    return <div>Loading session...</div>;
  }

  return <>{children}</>;
}
