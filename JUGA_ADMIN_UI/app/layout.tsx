import type { Metadata } from "next";
import { Inter, Plus_Jakarta_Sans } from "next/font/google";
import "./globals.css";

import { AppSidebar } from "@/components/layout/AppSidebar";
import { Header } from "@/components/layout/Header";
import { SidebarInset } from "@/components/ui/sidebar";
import { Toaster } from "@/components/ui/sonner";
import { Providers } from "@/providers/providers";
import { auth } from "@/auth";

const plusJakarta = Plus_Jakarta_Sans({
  subsets: ["latin"],
  variable: "--font-sans",
});

export const metadata: Metadata = {
  title: "AI Ajan Yönetim Paneli",
  description: "shadcn/ui ve Tailwind CSS v4 ile oluşturulmuş modern AI ajan yönetim paneli",
};

export default async function RootLayout({ children }: { children: React.ReactNode }) {
  const session = await auth();
  return (
    <html lang="tr" suppressHydrationWarning>
      <body className={`${plusJakarta.variable} font-sans antialiased`}>
        <Providers>
          <AppSidebar />
          <SidebarInset>
            <Header session={session} />
            {children}
            <Toaster richColors position="top-center" />
          </SidebarInset>
        </Providers>
      </body>
    </html>
  );
}
