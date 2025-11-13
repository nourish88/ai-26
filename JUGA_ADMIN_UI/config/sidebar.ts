// config/sidebar.ts
import { Home, AppWindow, Cpu, BadgePercent, Split, Unplug, FileStack, BarChart3, Activity } from "lucide-react";
import type { LucideIcon } from "lucide-react";

// You already had these types, they are perfect here.
export interface SubItem {
  title: string;
  url: string;
  badge?: string;
}

export interface SidebarLink {
  title: string;
  url?: string;
  icon: LucideIcon;
  badge?: string;
  subItems?: SubItem[];
}

export interface SidebarGroup {
  label: string;
  items: SidebarLink[];
}

export const sidebarConfig: SidebarGroup[] = [
  // ... your existing sidebarConfig array ...
  {
    label: "Genel",
    items: [
      { title: "Ana Panel", url: "/", icon: Home },
      { title: "Uygulamalar", url: "/apps", icon: AppWindow },
      { title: "Uygulama Sihirbazı", url: "/setup-wizard", icon: AppWindow },
    ],
  },
  {
    label: "Gözlemlenebilirlik",
    items: [{ title: "Uygulama İstatistikleri", url: "/observability", icon: Home }],
  },
  {
    label: "AI Altyapı Yönetimi",
    items: [
      {
        title: "LLM Yönetimi",
        icon: Cpu,
        subItems: [
          { title: "LLM Sağlayıcıları", url: "/providers" },
          { title: "Dil Modelleri (LLMs)", url: "/llms" },
          { title: "Embedding Modelleri", url: "/embeddings" },
        ],
      },
      {
        title: "Veri Çıkarım (Extraction) ",
        icon: BadgePercent,
        subItems: [
          { title: "Extraction Yönetimi", url: "/management" },
          { title: "Data Manager Yönetimi", url: "/data-manager" },
        ],
      },
      {
        title: "Parçalama (Chunking) Yönetimi",
        icon: Split,
        subItems: [
          { title: "Chunking Strategies", url: "/strategies" },
          { title: "Application Strategies", url: "/app-strategies" },
        ],
      },
      {
        title: "Arama Motoru",
        icon: Unplug,
        subItems: [
          { title: "Arama Motoru (Elastic)", url: "/search-engines" },
          { title: "App Search Engines", url: "/app-search-engines" },
        ],
      },
      {
        title: "Dosya Yönetimi",
        icon: Unplug,
        subItems: [
          { title: "File Store", url: "/file-store" },
          { title: "App File Store", url: "/app-file-store" },
        ],
      },
    ],
  },
  // {
  //   label: "İçerik ve Operasyon",
  //   items: [
  //     { title: "Dosya Yönetimi", icon: FileStack, url: "/files", badge: "Yeni" },
  //     { title: "Performans & Analitik", url: "/analytics", icon: BarChart3 },
  //     { title: "Sistem Logları", url: "/logs", icon: Activity },
  //   ],
  // },
];
