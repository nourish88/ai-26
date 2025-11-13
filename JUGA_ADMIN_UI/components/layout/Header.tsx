// components/layout/Header.tsx
"use client";

import { SidebarTrigger } from "@/components/ui/sidebar";
import { Separator } from "@/components/ui/separator";
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { Button } from "@/components/ui/button";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuLabel, DropdownMenuSeparator, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { Bell, Search, Moon, Sun, User, Settings, LogOut, HelpCircle } from "lucide-react";
import { useTheme } from "next-themes";
import { usePathname, useRouter } from "next/navigation";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { useEffect, useState } from "react";
import { signOut } from "next-auth/react"; // ✅ Import signOut
import LoginBtn from "../ui/login-btn";
import SignOut from "../ui/sign-out-button";
import { ModeToggle } from "../theme/ModeToggle";

interface HeaderProps {
  session: any;
}

export function Header({ session }: HeaderProps) {
  const { theme, setTheme } = useTheme();
  const pathname = usePathname();
  const router = useRouter();
  const [scrolled, setScrolled] = useState(false);

  // Generate breadcrumbs from pathname
  const breadcrumbs = pathname
    .split("/")
    .filter(Boolean)
    .map((segment) => ({
      label: segment
        .split("-")
        .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
        .join(" "),
      href: `/${segment}`,
    }));

  // Add scroll effect
  useEffect(() => {
    const handleScroll = () => {
      setScrolled(window.scrollY > 10);
    };
    window.addEventListener("scroll", handleScroll);
    return () => window.removeEventListener("scroll", handleScroll);
  }, []);

  // Get user initials
  const getUserInitials = (name?: string | null) => {
    if (!name) return "U";
    return name
      .split(" ")
      .map((n) => n[0])
      .join("")
      .toUpperCase()
      .slice(0, 2);
  };

  // ✅ Sign out handler
  const handleSignOut = async () => {
    await signOut({ callbackUrl: "/" });
  };

  // ✅ Sign in handler
  const handleSignIn = () => {
    router.push("/api/auth/signin");
  };

  return (
    <header className={cn("sticky top-0 z-40 flex h-16 shrink-0 items-center gap-2 transition-all duration-300", scrolled ? "bg-background/80 backdrop-blur-lg border-b border-border/50 shadow-sm" : "bg-background/50 backdrop-blur-sm")}>
      <div className="flex flex-1 items-center gap-2 px-4">
        {/* Sidebar Trigger */}
        <SidebarTrigger className="-ml-1 hover:bg-muted/50 transition-colors" />
        <Separator orientation="vertical" className="mr-2 h-4" />

        {/* Breadcrumb Navigation */}
        <div className="flex-1">
          <Breadcrumb>
            <BreadcrumbList>
              <BreadcrumbItem className="hidden md:block">
                <BreadcrumbLink href="/" className="flex items-center gap-2 hover:text-primary transition-colors group">
                  <span>Ana Sayfa</span>
                </BreadcrumbLink>
              </BreadcrumbItem>

              {breadcrumbs.map((crumb, index) => (
                <div key={crumb.href} className="flex items-center gap-2">
                  <BreadcrumbSeparator className="hidden md:block" />
                  <BreadcrumbItem>
                    {index === breadcrumbs.length - 1 ? (
                      <BreadcrumbPage className="font-semibold">{crumb.label}</BreadcrumbPage>
                    ) : (
                      <BreadcrumbLink href={crumb.href} className="hover:text-primary transition-colors">
                        {crumb.label}
                      </BreadcrumbLink>
                    )}
                  </BreadcrumbItem>
                </div>
              ))}
            </BreadcrumbList>
          </Breadcrumb>
        </div>

        {/* Right Side Actions */}
        <div className="flex items-center gap-2">
          {/* Search Button */}
          <Button variant="ghost" size="icon" className="relative hover:bg-muted/50 transition-all group">
            <Search className="h-4 w-4 group-hover:scale-110 transition-transform" />
            <span className="sr-only">Search</span>
          </Button>

          {/* Notifications */}
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <Button variant="ghost" size="icon" className="relative hover:bg-muted/50 transition-all group">
                <Bell className="h-4 w-4 group-hover:scale-110 transition-transform" />
                <Badge variant="destructive" className="absolute -top-1 -right-1 h-5 w-5 flex items-center justify-center p-0 text-xs">
                  3
                </Badge>
                <span className="sr-only">Notifications</span>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-80">
              <DropdownMenuLabel className="flex items-center justify-between">
                <span>Bildirimler</span>
                <Badge variant="secondary">3 Yeni</Badge>
              </DropdownMenuLabel>
              <DropdownMenuSeparator />
              <div className="max-h-[300px] overflow-y-auto">
                <DropdownMenuItem className="flex flex-col items-start gap-1 p-3 cursor-pointer">
                  <div className="flex items-center gap-2 w-full">
                    <div className="h-2 w-2 rounded-full bg-blue-500" />
                    <span className="font-medium text-sm">Yeni ajan oluşturuldu</span>
                    <span className="ml-auto text-xs text-muted-foreground">2dk önce</span>
                  </div>
                  <p className="text-xs text-muted-foreground pl-4">"Müşteri Destek Asistanı" başarıyla oluşturuldu</p>
                </DropdownMenuItem>
                <DropdownMenuItem className="flex flex-col items-start gap-1 p-3 cursor-pointer">
                  <div className="flex items-center gap-2 w-full">
                    <div className="h-2 w-2 rounded-full bg-green-500" />
                    <span className="font-medium text-sm">Ajan güncellendi</span>
                    <span className="ml-auto text-xs text-muted-foreground">1sa önce</span>
                  </div>
                  <p className="text-xs text-muted-foreground pl-4">LLM modeli başarıyla güncellendi</p>
                </DropdownMenuItem>
                <DropdownMenuItem className="flex flex-col items-start gap-1 p-3 cursor-pointer">
                  <div className="flex items-center gap-2 w-full">
                    <div className="h-2 w-2 rounded-full bg-yellow-500" />
                    <span className="font-medium text-sm">Sistem uyarısı</span>
                    <span className="ml-auto text-xs text-muted-foreground">3sa önce</span>
                  </div>
                  <p className="text-xs text-muted-foreground pl-4">API kullanım limiti %80'e ulaştı</p>
                </DropdownMenuItem>
              </div>
              <DropdownMenuSeparator />
              <DropdownMenuItem className="text-center text-sm text-primary cursor-pointer">Tümünü Görüntüle</DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>

          {/* Theme Toggle */}

          <ModeToggle />

          {/* Help */}
          <Button variant="ghost" size="icon" className="hover:bg-muted/50 transition-all group">
            <HelpCircle className="h-4 w-4 group-hover:scale-110 transition-transform" />
            <span className="sr-only">Help</span>
          </Button>

          <Separator orientation="vertical" className="h-4" />

          {/* User Menu */}
          {session?.user ? (
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button variant="ghost" className="relative h-9 w-9 rounded-full ring-2 ring-transparent hover:ring-primary/20 transition-all">
                  <Avatar className="h-9 w-9">
                    <AvatarImage src={session.user.image || undefined} alt={session.user.name || "User"} />
                    <AvatarFallback className="bg-gradient-to-br from-primary to-primary/50 text-primary-foreground">{getUserInitials(session.user.name)}</AvatarFallback>
                  </Avatar>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-56">
                <DropdownMenuLabel>
                  <div className="flex flex-col space-y-1">
                    <p className="text-sm font-medium leading-none">{session.user.name || "Kullanıcı"}</p>
                    <p className="text-xs leading-none text-muted-foreground">{session.user.email}</p>
                  </div>
                </DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem className="cursor-pointer">
                  <User className="mr-2 h-4 w-4" />
                  Profil
                </DropdownMenuItem>
                <DropdownMenuItem className="cursor-pointer">
                  <Settings className="mr-2 h-4 w-4" />
                  Ayarlar
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                {/* ✅ Sign Out with actual functionality */}

                <SignOut />
              </DropdownMenuContent>
            </DropdownMenu>
          ) : (
            <LoginBtn />
          )}
        </div>
      </div>
    </header>
  );
}
