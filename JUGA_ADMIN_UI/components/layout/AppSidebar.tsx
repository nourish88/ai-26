"use client";

import * as React from "react";
// next-auth/react'ten useSession hook'unu import ediyoruz
import { useSession } from "next-auth/react";

// Lucide ikonlarını import et
import { ChevronUp, User2, Settings, Activity, LogOut } from "lucide-react";

// UI Bileşenlerini import et
import { Sidebar, SidebarContent, SidebarFooter, SidebarGroup, SidebarGroupLabel, SidebarHeader, SidebarMenu, SidebarMenuItem, SidebarMenuButton, SidebarMenuSub, SidebarMenuSubButton, SidebarMenuSubItem } from "@/components/ui/sidebar";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { Badge } from "@/components/ui/badge";

// Yeni yapılandırmayı ve türleri import et
import { sidebarConfig, type SidebarLink, type SidebarGroup as SidebarGroupConfig } from "@/config/sidebar";
import Image from "next/image";
import Link from "next/link";
import { Button } from "@/components/ui/button"; // Oturum aç butonu için
import SignOutBtn from "../ui/sign-out-button"; // SignOutBtn bileşenini import et

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  // useSession hook'u ile oturum bilgilerini alıyoruz
  const { data: session } = useSession();

  const renderMenuItem = (item: SidebarLink) => {
    const hasSubItems = item.subItems && item.subItems.length > 0;

    if (hasSubItems) {
      return (
        <Collapsible key={item.title} asChild defaultOpen={false} className="group/collapsible">
          <SidebarMenuItem>
            <CollapsibleTrigger asChild>
              <SidebarMenuButton tooltip={item.title}>
                <item.icon />
                <span>{item.title}</span>
                {item.badge && (
                  <Badge variant="outline" className="ml-auto">
                    {item.badge}
                  </Badge>
                )}
                <ChevronUp className="ml-auto transition-transform duration-200 group-data-[state=open]/collapsible:rotate-180" />
              </SidebarMenuButton>
            </CollapsibleTrigger>
            <CollapsibleContent>
              <SidebarMenuSub>
                {item.subItems?.map((subItem) => (
                  <SidebarMenuSubItem key={subItem.title}>
                    <SidebarMenuSubButton asChild>
                      <a href={subItem.url}>
                        <span>{subItem.title}</span>
                        {subItem.badge && (
                          <Badge variant="secondary" className="ml-auto">
                            {subItem.badge}
                          </Badge>
                        )}
                      </a>
                    </SidebarMenuSubButton>
                  </SidebarMenuSubItem>
                ))}
              </SidebarMenuSub>
            </CollapsibleContent>
          </SidebarMenuItem>
        </Collapsible>
      );
    }

    return (
      <SidebarMenuItem key={item.title}>
        <SidebarMenuButton asChild tooltip={item.title}>
          <a href={item.url}>
            <item.icon />
            <span>{item.title}</span>
            {item.badge && (
              <Badge variant="secondary" className="ml-auto">
                {item.badge}
              </Badge>
            )}
          </a>
        </SidebarMenuButton>
      </SidebarMenuItem>
    );
  };

  return (
    <Sidebar collapsible="icon" {...props}>
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" asChild className="flex items-center justify-center">
              <div className="flex items-center justify-center rounded-lg bg-sidebar text-sidebar-primary-foreground">
                <Link href="/">
                  <Image src="/Beyaz ve Yazılı.svg" alt="logo" width={300} height={300} />
                </Link>
              </div>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>

      <SidebarContent>
        {sidebarConfig.map((group: SidebarGroupConfig) => (
          <SidebarGroup key={group.label}>
            <SidebarGroupLabel>{group.label}</SidebarGroupLabel>
            <SidebarMenu>{group.items.map((item: SidebarLink) => renderMenuItem(item))}</SidebarMenu>
          </SidebarGroup>
        ))}
      </SidebarContent>

      <SidebarFooter>
        <SidebarMenu>
          <SidebarMenuItem>
            {/* --- DİNAMİK KULLANICI BÖLÜMÜ --- */}
            {session?.user ? (
              // Eğer oturum açıksa, kullanıcı bilgilerini ve menüyü göster
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <SidebarMenuButton size="lg" className="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground">
                    <div className="flex aspect-square size-8 items-center justify-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground">
                      {/* Opsiyonel: Kullanıcı resmi varsa gösterilebilir */}
                      {/* {session.user.image ? <Image src={session.user.image} alt="Avatar" width={32} height={32} className="rounded-full" /> : <User2 className="size-4" />} */}
                      <User2 className="size-4" />
                    </div>
                    <div className="grid flex-1 text-left text-sm leading-tight">
                      {/* session.user.name null/undefined kontrolü */}
                      <span className="truncate font-semibold">{session.user.name ?? "Kullanıcı"}</span>
                      <span className="truncate text-xs">{session.user.email}</span>
                    </div>
                    <ChevronUp className="ml-auto size-4" />
                  </SidebarMenuButton>
                </DropdownMenuTrigger>
                <DropdownMenuContent className="w-[--radix-dropdown-menu-trigger-width] min-w-56 rounded-lg" side="top" align="end" sideOffset={4}>
                  <DropdownMenuItem>
                    <User2 className="mr-2 h-4 w-4" />
                    Profil Ayarları
                  </DropdownMenuItem>
                  <DropdownMenuItem>
                    <Settings className="mr-2 h-4 w-4" />
                    Sistem Ayarları
                  </DropdownMenuItem>
                  <DropdownMenuItem>
                    <Activity className="mr-2 h-4 w-4" />
                    Aktivite Logları
                  </DropdownMenuItem>
                  <DropdownMenuItem asChild>
                    {/* SignOutBtn'i DropdownMenuItem içinde kullanmak daha iyi erişilebilirlik sağlar */}
                    <SignOutBtn />
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            ) : (
              // Eğer oturum açık değilse, bir "Giriş Yap" linki göster
              <SidebarMenuButton asChild>
                <Link href="/api/auth/signin">
                  <User2 className="size-4" />
                  <span>Giriş Yap</span>
                </Link>
              </SidebarMenuButton>
            )}
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarFooter>
      {/* SidebarRail kaldırıldı, eğer ihtiyacınız varsa geri ekleyebilirsiniz. */}
    </Sidebar>
  );
}
