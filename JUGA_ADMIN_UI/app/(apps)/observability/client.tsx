// app/(apps)/observability/page.tsx
"use client";

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Activity, Clock, DollarSign, Zap, CheckCircle2, XCircle, RefreshCw, AlertTriangle, ArrowUpRight, ArrowDownRight, Download, Bell, Target, Filter } from "lucide-react";
import { useState, useEffect, useMemo } from "react";
import { toast } from "sonner";
import { cn } from "@/lib/utils";
import { Area, AreaChart, Bar, BarChart, CartesianGrid, Line, LineChart, Pie, PieChart, XAxis, YAxis } from "recharts";
import { ChartConfig, ChartContainer, ChartTooltip, ChartTooltipContent, ChartLegend, ChartLegendContent } from "@/components/ui/chart";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";

interface Metrics {
  totalTraces: number;
  totalTokens: number;
  totalCost: number;
  avgLatency: number;
  successRate: number;
  errorRate: number;
  activeUsers: number;
  totalSessions: number;
  latencyPercentiles: { p50: number; p95: number; p99: number };
  topAgents: Array<{
    name: string;
    requests: number;
    tokens: number;
    cost: number;
    successRate: number;
    avgLatency: number;
  }>;
  recentErrors: Array<{
    id: string;
    timestamp: string;
    agent: string;
    error: string;
  }>;
  totalErrors: number;
  _mock?: boolean;
}

// Mock time series data - replace with real API data
const generateTimeSeriesData = (days: number) => {
  const data = [];
  const now = new Date();
  for (let i = days; i >= 0; i--) {
    const date = new Date(now);
    date.setDate(date.getDate() - i);
    data.push({
      date: date.toISOString().split("T")[0],
      requests: Math.floor(Math.random() * 300) + 200,
      errors: Math.floor(Math.random() * 15) + 2,
      p50: Math.floor(Math.random() * 100) + 150,
      p95: Math.floor(Math.random() * 150) + 300,
      p99: Math.floor(Math.random() * 200) + 450,
    });
  }
  return data;
};

const requestsChartConfig = {
  requests: {
    label: "İstekler",
    color: "var(--chart-1)",
  },
  errors: {
    label: "Hatalar",
    color: "var(--chart-2)",
  },
} satisfies ChartConfig;

const latencyChartConfig = {
  p50: {
    label: "P50",
    color: "var(--chart-3)",
  },
  p95: {
    label: "P95",
    color: "var(--chart-4)",
  },
  p99: {
    label: "P99",
    color: "var(--chart-5)",
  },
} satisfies ChartConfig;

export default function ObservabilityPage() {
  const [timeRange, setTimeRange] = useState<"1h" | "24h" | "7d" | "30d">("24h");
  const [chartTimeRange, setChartTimeRange] = useState("30d");
  const [metrics, setMetrics] = useState<Metrics | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [lastUpdated, setLastUpdated] = useState<Date | null>(null);
  const [activeLatencyChart, setActiveLatencyChart] = useState<"p50" | "p95" | "p99">("p50");

  // Generate mock time series data
  const allTimeSeriesData = useMemo(() => generateTimeSeriesData(90), []);

  const filteredData = useMemo(() => {
    const days = chartTimeRange === "90d" ? 90 : chartTimeRange === "30d" ? 30 : 7;
    return allTimeSeriesData.slice(-days);
  }, [chartTimeRange, allTimeSeriesData]);

  useEffect(() => {
    fetchMetrics();
    const interval = setInterval(fetchMetrics, 30000);
    return () => clearInterval(interval);
  }, [timeRange]);

  const fetchMetrics = async () => {
    setIsLoading(true);
    try {
      const response = await fetch(`/api/observability/metrics?range=${timeRange}`);

      if (!response.ok) {
        throw new Error("Failed to fetch");
      }

      const data = await response.json();
      setMetrics(data);
      setLastUpdated(new Date());

      if (data._mock) {
        toast.warning("Demo Veri Kullanılıyor", {
          description: "LangFuse'a bağlanılamadı",
        });
      }
    } catch (error) {
      console.error("Error:", error);
      toast.error("Veri yüklenemedi");
    } finally {
      setIsLoading(false);
    }
  };

  const formatNumber = (num: number) => new Intl.NumberFormat("tr-TR").format(num);
  const formatCurrency = (num: number) => new Intl.NumberFormat("tr-TR", { style: "currency", currency: "USD" }).format(num);

  const getHealthStatus = (rate: number) => {
    if (rate >= 99) return { label: "Mükemmel", color: "text-green-600", icon: CheckCircle2 };
    if (rate >= 95) return { label: "İyi", color: "text-green-600", icon: CheckCircle2 };
    if (rate >= 90) return { label: "Normal", color: "text-yellow-600", icon: AlertTriangle };
    return { label: "Kritik", color: "text-red-600", icon: XCircle };
  };

  const health = getHealthStatus(metrics?.successRate || 0);
  const HealthIcon = health.icon;

  const trends = {
    requests: 12.5,
    tokens: -3.2,
    cost: 8.1,
    latency: -15.3,
  };

  // Prepare agent pie chart data
  const agentPieData = useMemo(() => {
    return (
      metrics?.topAgents.slice(0, 5).map((agent, idx) => ({
        name: agent.name,
        value: agent.requests,
        fill: `var(--chart-${(idx % 5) + 1})`,
      })) || []
    );
  }, [metrics]);

  const agentPieConfig = {
    value: {
      label: "İstekler",
    },
    ...Object.fromEntries(
      agentPieData.map((agent, idx) => [
        agent.name,
        {
          label: agent.name,
          color: `var(--chart-${(idx % 5) + 1})`,
        },
      ])
    ),
  } satisfies ChartConfig;

  // Calculate latency totals for interactive chart
  const latencyTotals = useMemo(() => {
    return {
      p50: filteredData.reduce((acc, curr) => acc + curr.p50, 0),
      p95: filteredData.reduce((acc, curr) => acc + curr.p95, 0),
      p99: filteredData.reduce((acc, curr) => acc + curr.p99, 0),
    };
  }, [filteredData]);

  return (
    <div className="flex-1 space-y-6 p-6">
      {/* ============= HEADER ============= */}
      <div className="flex flex-col lg:flex-row lg:items-center justify-between gap-4">
        <div className="space-y-1">
          <h1 className="text-3xl font-bold tracking-tight">Gözlemlenebilirlik</h1>
          <div className="flex items-center gap-4 text-sm text-muted-foreground">
            <div className="flex items-center gap-2">
              <div className="h-2 w-2 rounded-full bg-green-500 animate-pulse" />
              <span>Canlı</span>
            </div>
            {lastUpdated && (
              <>
                <span>•</span>
                <span>Son güncelleme: {lastUpdated.toLocaleTimeString("tr-TR")}</span>
              </>
            )}
          </div>
        </div>

        <div className="flex items-center gap-2 flex-wrap">
          <div className="flex items-center gap-1 p-1 bg-muted rounded-lg">
            {(["1h", "24h", "7d", "30d"] as const).map((range) => (
              <Button key={range} variant={timeRange === range ? "default" : "ghost"} size="sm" onClick={() => setTimeRange(range)}>
                {range === "1h" ? "1 Saat" : range === "24h" ? "24 Saat" : range === "7d" ? "7 Gün" : "30 Gün"}
              </Button>
            ))}
          </div>

          <Button variant="outline" size="sm" onClick={fetchMetrics} disabled={isLoading} className="gap-2">
            <RefreshCw className={cn("h-4 w-4", isLoading && "animate-spin")} />
            Yenile
          </Button>

          <Button variant="outline" size="sm" className="gap-2">
            <Download className="h-4 w-4" />
            Dışa Aktar
          </Button>

          <Button variant="outline" size="sm" className="gap-2">
            <Bell className="h-4 w-4" />
            Uyarılar
          </Button>
        </div>
      </div>

      {/* ============= FILTERS ============= */}

      {/* ============= SYSTEM HEALTH ============= */}
      <Card className={cn("border-2", metrics?.successRate! >= 95 ? "border-green-500/50 bg-green-50/50 dark:bg-green-950/20" : "border-yellow-500/50 bg-yellow-50/50 dark:bg-yellow-950/20")}>
        <CardContent className="pt-6">
          <div className="space-y-4">
            <div className="flex items-center gap-4">
              <div className={cn("h-12 w-12 rounded-full flex items-center justify-center", metrics?.successRate! >= 95 ? "bg-green-100 dark:bg-green-950" : "bg-yellow-100 dark:bg-yellow-950")}>
                <HealthIcon className={cn("h-6 w-6", health.color)} />
              </div>
              <div className="flex-1">
                <div className="flex items-center gap-2">
                  <h3 className="text-xl font-semibold">Sistem Durumu: {health.label}</h3>
                  <Badge variant="outline" className={health.color}>
                    {metrics?.successRate}% Başarılı
                  </Badge>
                </div>
                <p className="text-sm text-muted-foreground mt-1">
                  {metrics?.totalTraces ? formatNumber(metrics.totalTraces) : "0"} istek • {metrics?.totalErrors || 0} hata • {metrics?.avgLatency || 0}ms ortalama yanıt
                </p>
              </div>
            </div>

            <div className="space-y-2">
              <div className="flex items-center justify-between text-xs">
                <span className="text-muted-foreground">Başarı Oranı</span>
                <span className="font-medium">{metrics?.successRate}%</span>
              </div>
              <div className="h-2 bg-muted rounded-full overflow-hidden">
                <div className="h-full bg-gradient-to-r from-green-500 to-green-600 rounded-full transition-all duration-500" style={{ width: `${metrics?.successRate || 0}%` }} />
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* ============= KEY METRICS ============= */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card className="hover:shadow-lg transition-all">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Toplam İstek</CardTitle>
            <Activity className="h-4 w-4 text-blue-600" />
          </CardHeader>
          <CardContent>
            <div className="space-y-1">
              <div className="flex items-baseline gap-2">
                <div className="text-2xl font-bold">{isLoading ? "..." : formatNumber(metrics?.totalTraces || 0)}</div>
                <div className={cn("flex items-center gap-1 text-xs font-medium", trends.requests > 0 ? "text-green-600" : "text-red-600")}>
                  {trends.requests > 0 ? <ArrowUpRight className="h-3 w-3" /> : <ArrowDownRight className="h-3 w-3" />}
                  <span>{Math.abs(trends.requests)}%</span>
                </div>
              </div>
              <p className="text-xs text-muted-foreground">{formatNumber(metrics?.activeUsers || 0)} aktif kullanıcı</p>
            </div>
          </CardContent>
        </Card>

        <Card className="hover:shadow-lg transition-all">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Token Kullanımı</CardTitle>
            <Zap className="h-4 w-4 text-yellow-600" />
          </CardHeader>
          <CardContent>
            <div className="space-y-1">
              <div className="flex items-baseline gap-2">
                <div className="text-2xl font-bold">{isLoading ? "..." : `${((metrics?.totalTokens || 0) / 1000000).toFixed(1)}M`}</div>
                <div className={cn("flex items-center gap-1 text-xs font-medium", trends.tokens > 0 ? "text-green-600" : "text-red-600")}>
                  {trends.tokens > 0 ? <ArrowUpRight className="h-3 w-3" /> : <ArrowDownRight className="h-3 w-3" />}
                  <span>{Math.abs(trends.tokens)}%</span>
                </div>
              </div>
              <p className="text-xs text-muted-foreground">~{formatNumber(Math.round((metrics?.totalTokens || 0) / (metrics?.totalTraces || 1)))} token/istek</p>
            </div>
          </CardContent>
        </Card>

        <Card className="hover:shadow-lg transition-all">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Toplam Maliyet</CardTitle>
            <DollarSign className="h-4 w-4 text-green-600" />
          </CardHeader>
          <CardContent>
            <div className="space-y-1">
              <div className="flex items-baseline gap-2">
                <div className="text-2xl font-bold">{isLoading ? "..." : formatCurrency(metrics?.totalCost || 0)}</div>
                <div className={cn("flex items-center gap-1 text-xs font-medium", trends.cost > 0 ? "text-red-600" : "text-green-600")}>
                  {trends.cost > 0 ? <ArrowUpRight className="h-3 w-3" /> : <ArrowDownRight className="h-3 w-3" />}
                  <span>{Math.abs(trends.cost)}%</span>
                </div>
              </div>
              <p className="text-xs text-muted-foreground">{formatCurrency((metrics?.totalCost || 0) / (metrics?.totalTraces || 1))} per istek</p>
            </div>
          </CardContent>
        </Card>

        <Card className="hover:shadow-lg transition-all">
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Yanıt Süresi</CardTitle>
            <Clock className="h-4 w-4 text-purple-600" />
          </CardHeader>
          <CardContent>
            <div className="space-y-1">
              <div className="flex items-baseline gap-2">
                <div className="text-2xl font-bold">{isLoading ? "..." : `${metrics?.avgLatency || 0}ms`}</div>
                <div className={cn("flex items-center gap-1 text-xs font-medium", trends.latency < 0 ? "text-green-600" : "text-red-600")}>
                  {trends.latency < 0 ? <ArrowDownRight className="h-3 w-3" /> : <ArrowUpRight className="h-3 w-3" />}
                  <span>{Math.abs(trends.latency)}%</span>
                </div>
              </div>
              <p className="text-xs text-muted-foreground">P95: {metrics?.latencyPercentiles.p95 || 0}ms</p>
            </div>
          </CardContent>
        </Card>
      </div>
      {/* ============= AGENT DISTRIBUTION & ERRORS ============= */}
      <div className="grid gap-6 lg:grid-cols-2">
        {/* Pie Chart - Agent Distribution */}
        <Card className="flex flex-col">
          <CardHeader className="items-center pb-0">
            <CardTitle>Ajan Dağılımı</CardTitle>
            <CardDescription>İstek sayısına göre top 5 ajan</CardDescription>
          </CardHeader>
          <CardContent className="flex-1 pb-0">
            {isLoading ? (
              <div className="flex items-center justify-center h-[250px]">
                <RefreshCw className="h-8 w-8 animate-spin text-muted-foreground" />
              </div>
            ) : agentPieData.length > 0 ? (
              <ChartContainer config={agentPieConfig} className="mx-auto aspect-square max-h-[250px]">
                <PieChart>
                  <ChartTooltip content={<ChartTooltipContent hideLabel />} />
                  <Pie data={agentPieData} dataKey="value" label nameKey="name" />
                </PieChart>
              </ChartContainer>
            ) : (
              <div className="flex flex-col items-center justify-center h-[250px]">
                <Activity className="h-12 w-12 text-muted-foreground mb-2 opacity-50" />
                <p className="text-sm font-medium text-muted-foreground">Henüz veri yok</p>
              </div>
            )}
          </CardContent>
        </Card>

        {/* Recent Errors */}
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <AlertTriangle className="h-5 w-5 text-red-600" />
              Son Hatalar ({metrics?.totalErrors || 0})
            </CardTitle>
            <CardDescription>Son {timeRange} içindeki hatalar</CardDescription>
          </CardHeader>
          <CardContent>
            {isLoading ? (
              <div className="space-y-3">
                {[1, 2, 3].map((i) => (
                  <div key={i} className="flex items-center gap-3 p-3 rounded-lg border">
                    <div className="flex-1 space-y-2">
                      <div className="h-4 bg-muted animate-pulse rounded w-3/4" />
                      <div className="h-3 bg-muted animate-pulse rounded w-1/2" />
                    </div>
                    <div className="w-4 h-4 bg-muted animate-pulse rounded-full" />
                  </div>
                ))}
              </div>
            ) : metrics?.recentErrors && metrics.recentErrors.length > 0 ? (
              <div className="space-y-3 max-h-[268px] overflow-y-auto">
                {metrics.recentErrors.map((error) => (
                  <div key={error.id} className="p-3 rounded-lg border border-red-200 dark:border-red-900 bg-red-50/50 dark:bg-red-950/20">
                    <div className="flex items-start justify-between gap-2">
                      <div className="flex-1 min-w-0">
                        <p className="font-medium text-sm truncate">{error.error}</p>
                        <p className="text-xs text-muted-foreground mt-1">
                          {error.agent} • {new Date(error.timestamp).toLocaleString("tr-TR")}
                        </p>
                      </div>
                      <XCircle className="h-4 w-4 text-red-600 flex-shrink-0" />
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="text-center py-12">
                <CheckCircle2 className="h-12 w-12 text-green-600 mx-auto mb-2" />
                <p className="text-sm font-medium">Harika! Hata yok</p>
                <p className="text-xs text-muted-foreground mt-1">Tüm sistemler normal çalışıyor</p>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
      {/* ============= INTERACTIVE AREA CHART - REQUESTS ============= */}
      <Card className="pt-0">
        <CardHeader className="flex items-center gap-2 space-y-0 border-b py-5 sm:flex-row">
          <div className="grid flex-1 gap-1">
            <CardTitle>İstek ve Hata Trendleri</CardTitle>
            <CardDescription>Zaman içinde toplam istek ve hata sayıları</CardDescription>
          </div>
          <Select value={chartTimeRange} onValueChange={setChartTimeRange}>
            <SelectTrigger className="w-[160px] rounded-lg" aria-label="Zaman aralığı seç">
              <SelectValue placeholder="Son 3 ay" />
            </SelectTrigger>
            <SelectContent className="rounded-xl">
              <SelectItem value="90d" className="rounded-lg">
                Son 3 ay
              </SelectItem>
              <SelectItem value="30d" className="rounded-lg">
                Son 30 gün
              </SelectItem>
              <SelectItem value="7d" className="rounded-lg">
                Son 7 gün
              </SelectItem>
            </SelectContent>
          </Select>
        </CardHeader>
        <CardContent className="px-2 pt-4 sm:px-6 sm:pt-6">
          <ChartContainer config={requestsChartConfig} className="aspect-auto h-[300px] w-full">
            <AreaChart data={filteredData}>
              <defs>
                <linearGradient id="fillRequests" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="var(--color-requests)" stopOpacity={0.8} />
                  <stop offset="95%" stopColor="var(--color-requests)" stopOpacity={0.1} />
                </linearGradient>
                <linearGradient id="fillErrors" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="var(--color-errors)" stopOpacity={0.8} />
                  <stop offset="95%" stopColor="var(--color-errors)" stopOpacity={0.1} />
                </linearGradient>
              </defs>
              <CartesianGrid vertical={false} />
              <XAxis
                dataKey="date"
                tickLine={false}
                axisLine={false}
                tickMargin={8}
                minTickGap={32}
                tickFormatter={(value) => {
                  const date = new Date(value);
                  return date.toLocaleDateString("tr-TR", {
                    month: "short",
                    day: "numeric",
                  });
                }}
              />
              <ChartTooltip
                cursor={false}
                content={
                  <ChartTooltipContent
                    labelFormatter={(value) => {
                      return new Date(value).toLocaleDateString("tr-TR", {
                        month: "long",
                        day: "numeric",
                      });
                    }}
                    indicator="dot"
                  />
                }
              />
              <Area dataKey="errors" type="natural" fill="url(#fillErrors)" stroke="var(--color-errors)" stackId="a" />
              <Area dataKey="requests" type="natural" fill="url(#fillRequests)" stroke="var(--color-requests)" stackId="a" />
              <ChartLegend content={<ChartLegendContent />} />
            </AreaChart>
          </ChartContainer>
        </CardContent>
      </Card>

      {/* ============= RESPONSE TIME BREAKDOWN - CLEARER VERSION ============= */}
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle>Yanıt Süresi Dağılımı</CardTitle>
              <CardDescription>Kullanıcılarınız tipik olarak ne kadar sürede yanıt alıyor?</CardDescription>
            </div>
            <Badge variant="outline" className="gap-2">
              <Target className="h-3 w-3" />
              Hedef: &lt;500ms
            </Badge>
          </div>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            {/* P50 - Typical Experience */}
            <div className="space-y-3 p-4 rounded-lg border bg-gradient-to-br from-green-50 to-green-50/50 dark:from-green-950/20 dark:to-green-950/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-muted-foreground">Tipik Deneyim</p>
                  <p className="text-xs text-muted-foreground mt-1">Çoğu kullanıcı bu kadar hızlı</p>
                </div>
                <Badge variant="outline" className="text-green-600 border-green-600">
                  P50
                </Badge>
              </div>
              <div className="flex items-baseline gap-2">
                <p className="text-3xl font-bold">{metrics?.latencyPercentiles.p50 || 0}ms</p>
                <span className="text-xs text-green-600 font-medium">✓ Hızlı</span>
              </div>
              <div className="h-2 bg-muted rounded-full overflow-hidden">
                <div className="h-full bg-gradient-to-r from-green-500 to-green-600 rounded-full transition-all" style={{ width: `${Math.min(((metrics?.latencyPercentiles.p50 || 0) / 500) * 100, 100)}%` }} />
              </div>
              <p className="text-xs text-muted-foreground">Tüm isteklerin %50'si bu hızda, %50'si daha yavaş</p>
            </div>

            {/* P95 - Almost All Users */}
            <div className="space-y-3 p-4 rounded-lg border bg-gradient-to-br from-yellow-50 to-yellow-50/50 dark:from-yellow-950/20 dark:to-yellow-950/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-muted-foreground">Çoğu Kullanıcı</p>
                  <p className="text-xs text-muted-foreground mt-1">19 de 20 kullanıcı bu kadar hızlı</p>
                </div>
                <Badge variant="outline" className="text-yellow-600 border-yellow-600">
                  P95
                </Badge>
              </div>
              <div className="flex items-baseline gap-2">
                <p className="text-3xl font-bold">{metrics?.latencyPercentiles.p95 || 0}ms</p>
                <span className="text-xs text-yellow-600 font-medium">~ Normal</span>
              </div>
              <div className="h-2 bg-muted rounded-full overflow-hidden">
                <div className="h-full bg-gradient-to-r from-yellow-500 to-yellow-600 rounded-full transition-all" style={{ width: `${Math.min(((metrics?.latencyPercentiles.p95 || 0) / 500) * 100, 100)}%` }} />
              </div>
              <p className="text-xs text-muted-foreground">Tüm isteklerin %95'si bu hızda, sadece %5'i daha yavaş</p>
            </div>

            {/* P99 - Worst Case Scenario */}
            <div className="space-y-3 p-4 rounded-lg border bg-gradient-to-br from-red-50 to-red-50/50 dark:from-red-950/20 dark:to-red-950/10">
              <div className="flex items-center justify-between">
                <div>
                  <p className="text-sm font-medium text-muted-foreground">En Kötü Durum</p>
                  <p className="text-xs text-muted-foreground mt-1">Sadece 1/100 kullanıcı bu kadar yavaş</p>
                </div>
                <Badge variant="outline" className="text-red-600 border-red-600">
                  P99
                </Badge>
              </div>
              <div className="flex items-baseline gap-2">
                <p className="text-3xl font-bold">{metrics?.latencyPercentiles.p99 || 0}ms</p>
                <span className="text-xs text-red-600 font-medium">⚠ Yavaş</span>
              </div>
              <div className="h-2 bg-muted rounded-full overflow-hidden">
                <div className="h-full bg-gradient-to-r from-red-500 to-red-600 rounded-full transition-all" style={{ width: `${Math.min(((metrics?.latencyPercentiles.p99 || 0) / 500) * 100, 100)}%` }} />
              </div>
              <p className="text-xs text-muted-foreground">Tüm isteklerin %99'u bu hızda, sadece %1'i daha yavaş</p>
            </div>
          </div>

          {/* Explanation Section */}
          <div className="mt-6 pt-6 border-t">
            <h4 className="text-sm font-semibold mb-3">Bu Metrikler Neden Önemli?</h4>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-sm">
              <div className="space-y-2">
                <p className="font-medium">P50 vs Ortalama</p>
                <p className="text-muted-foreground">P50, ortalamadan daha gerçekçi bir görüntü verir. Birkaç çok yavaş istek, ortalamayı yükselterek yanıltabilir.</p>
              </div>
              <div className="space-y-2">
                <p className="font-medium">P95 ile Uyarı Alın</p>
                <p className="text-muted-foreground">P95'te ciddi yavaşlama varsa, çoğu kullanıcı etkileniyordur. Bu hemen dikkat gerektiren bir işarettir.</p>
              </div>
              <div className="space-y-2">
                <p className="font-medium">P99 Optimize Edin</p>
                <p className="text-muted-foreground">P99 yüksekse, kritik kullanıcılar (ödeme yapanlar) etkilenebilir. Hata çıkışlarını araştırın.</p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
