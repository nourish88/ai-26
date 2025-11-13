// app/(dashboard)/page.tsx
import { AgentsTable } from "@/components/dashboard/AgentsTable";
import { AnalyticsChart } from "@/components/dashboard/AnalyticsChart";
import { DashboardStats } from "@/components/dashboard/DashboardStats";
import { RecentActivity } from "@/components/dashboard/RecentActivity";
import { Breadcrumb, BreadcrumbList, BreadcrumbItem, BreadcrumbLink, BreadcrumbPage, BreadcrumbSeparator } from "@/components/ui/breadcrumb";
import { SidebarTrigger } from "@/components/ui/sidebar";
import { Separator } from "@/components/ui/separator";
import { Button } from "@/components/ui/button";
import { Plus, Settings } from "lucide-react";

export default function DashboardPage() {
  return (
    <main className="flex-1 overflow-x-hidden overflow-y-auto bg-background">
      {/* ============= TOP NAVIGATION BAR ============= */}

      {/* ============= MAIN CONTENT ============= */}
      <div className="px-6 py-8">
        <div className="mx-auto max-w-7xl space-y-8">
          {/* ============= HEADER SECTION ============= */}
          <div className="space-y-2">
            <h1 className="text-3xl font-bold tracking-tight text-foreground">Sihirbaz Kontrol Paneli</h1>
            <p className="text-base text-muted-foreground">Hoş geldiniz! İşte bugün AI ajanlarınızla ilgili son durumlar ve performans metrikleri.</p>
          </div>

          {/* ============= STATS SECTION ============= */}
          <section className="space-y-4">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-semibold">Genel Bakış</h2>
            </div>
            <DashboardStats />
          </section>

          {/* ============= CHARTS & ACTIVITY SECTION ============= */}
          <section className="space-y-4">
            <h2 className="text-lg font-semibold">Analitik & Aktivite</h2>
            <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
              {/* Analytics Chart - Takes up 2 columns on desktop */}
              <div className="lg:col-span-2">
                <AnalyticsChart />
              </div>

              {/* Recent Activity - Takes up 1 column on desktop */}
              <div className="lg:col-span-1">
                <RecentActivity />
              </div>
            </div>
          </section>

          {/* ============= AGENTS TABLE SECTION ============= */}
          <section className="space-y-4">
            <div className="flex items-center justify-between">
              <div>
                <h2 className="text-lg font-semibold">Ajanlar</h2>
                <p className="text-sm text-muted-foreground mt-1">Tüm AI ajanlarınızın durumu ve istatistikleri</p>
              </div>
              <Button size="sm" className="gap-2">
                <Plus className="h-4 w-4" />
                Ajan Ekle
              </Button>
            </div>
            <AgentsTable />
          </section>
        </div>
      </div>
    </main>
  );
}
