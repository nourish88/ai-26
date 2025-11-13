// app/providers.tsx
"use client";

import { ThemeProvider } from "@/components/theme/ThemeProvider";
import { SidebarProvider } from "@/components/ui/sidebar";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";

import { SessionProvider } from "next-auth/react";

import { useState } from "react";
import { SessionGuard } from "./SessionGuard";

export function Providers({ children }: { children: React.ReactNode }) {
  const [queryClient] = useState(
    () =>
      new QueryClient({
        defaultOptions: {
          queries: {
            // Stale time - data considered fresh for 5 minutes
            staleTime: 5 * 60 * 1000,
            // Cache time - keep in cache for 10 minutes after becoming unused
            gcTime: 10 * 60 * 1000,
            // Retry failed requests once
            retry: 1,
            // Don't refetch on window focus by default
            refetchOnWindowFocus: false,
          },
          mutations: {
            // Don't retry mutations by default
            retry: 0,
          },
        },
      })
  );

  return (
    <SessionProvider>
      {/* <SessionGuard> */}
      <QueryClientProvider client={queryClient}>
        <SidebarProvider>
          <ThemeProvider attribute="class" defaultTheme="system" enableSystem disableTransitionOnChange>
            {children}
          </ThemeProvider>
        </SidebarProvider>
        {/* Show DevTools in development */}
        {/* {process.env.NODE_ENV === "development" && <ReactQueryDevtools initialIsOpen={false} />} */}
      </QueryClientProvider>
      {/* </SessionGuard> */}
    </SessionProvider>
  );
}
