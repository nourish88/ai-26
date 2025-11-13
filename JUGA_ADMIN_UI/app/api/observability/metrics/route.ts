// app/api/observability/metrics/route.ts
import { NextRequest, NextResponse } from "next/server";

function formatAgentName(tagName: string): string {
  // Remove common prefixes like "agent:", "app:", etc.
  let name = tagName.replace(/^(agent|app|service):/i, "");

  // Convert kebab-case or snake_case to Title Case
  name = name
    .split(/[-_]/)
    .map((word) => word.charAt(0).toLocaleUpperCase("tr-TR") + word.slice(1).toLocaleLowerCase("tr-TR"))
    .join(" ");

  return name;
}

const LANGFUSE_BASE_URL = process.env.LANGFUSE_BASE_URL || "http://127.0.0.1:3050";
const LANGFUSE_PUBLIC_KEY = process.env.LANGFUSE_PUBLIC_KEY!;
const LANGFUSE_SECRET_KEY = process.env.LANGFUSE_SECRET_KEY!;

async function callLangFuseMetrics(queryObj: any) {
  const queryString = encodeURIComponent(JSON.stringify(queryObj));
  const url = `${LANGFUSE_BASE_URL}/api/public/metrics?query=${queryString}`;

  const response = await fetch(url, {
    method: "GET",
    headers: {
      Authorization: `Basic ${Buffer.from(`${LANGFUSE_PUBLIC_KEY}:${LANGFUSE_SECRET_KEY}`).toString("base64")}`,
    },
  });

  if (!response.ok) {
    const error = await response.text();
    throw new Error(`LangFuse error (${response.status}): ${error}`);
  }

  return response.json();
}

export async function GET(request: NextRequest) {
  try {
    const searchParams = request.nextUrl.searchParams;
    const range = searchParams.get("range") || "24h";

    const now = new Date();
    const startDate = new Date();

    switch (range) {
      case "1h":
        startDate.setHours(startDate.getHours() - 1);
        break;
      case "24h":
        startDate.setHours(startDate.getHours() - 24);
        break;
      case "7d":
        startDate.setDate(startDate.getDate() - 7);
        break;
      case "30d":
        startDate.setDate(startDate.getDate() - 30);
        break;
    }

    const fromTimestamp = startDate.toISOString();
    const toTimestamp = now.toISOString();

    console.log("🔍 Fetching metrics:", { range, fromTimestamp, toTimestamp });

    // ============= QUERY 1: Overall Trace Metrics =============
    const overallQuery = {
      view: "traces",
      metrics: [
        { measure: "count", aggregation: "count" },
        { measure: "totalTokens", aggregation: "sum" },
        { measure: "totalCost", aggregation: "sum" },
        { measure: "latency", aggregation: "avg" },
      ],
      dimensions: [],
      filters: [],
      fromTimestamp,
      toTimestamp,
    };

    const overallData = await callLangFuseMetrics(overallQuery);
    const overall = overallData.data?.[0] || {};

    // ============= QUERY 2: Latency Percentiles =============
    const latencyQuery = {
      view: "traces",
      metrics: [
        { measure: "latency", aggregation: "p50" },
        { measure: "latency", aggregation: "p95" },
        { measure: "latency", aggregation: "p99" },
      ],
      dimensions: [],
      filters: [],
      fromTimestamp,
      toTimestamp,
    };

    const latencyData = await callLangFuseMetrics(latencyQuery);
    const latency = latencyData.data?.[0] || {};

    // ============= QUERY 3: Error Count (using observations with level filter) =============
    // 🔥 FIX: Use "observations" view for level filtering
    const errorQuery = {
      view: "observations",
      metrics: [{ measure: "count", aggregation: "count" }],
      dimensions: [],
      filters: [{ column: "level", operator: "=", value: "ERROR", type: "string" }],
      fromTimestamp,
      toTimestamp,
    };

    let errorCount = 0;
    try {
      const errorData = await callLangFuseMetrics(errorQuery);
      console.log("errorData", errorData);
      errorCount = parseInt(errorData.data?.[0]?.count_count || "0");
    } catch (err) {
      console.warn("Could not fetch error count:", err);
    }

    const agentsQuery = {
      view: "traces",
      metrics: [
        { measure: "count", aggregation: "count" },
        { measure: "totalTokens", aggregation: "sum" },
        { measure: "totalCost", aggregation: "sum" },
        { measure: "latency", aggregation: "avg" },
      ],
      dimensions: [{ field: "tags" }], // ✅ Changed from "name" to "tags"
      filters: [],
      fromTimestamp,
      toTimestamp,
      orderBy: [{ field: "count_count", direction: "desc" }],
      config: {
        row_limit: 10,
      },
    };

    const agentsData = await callLangFuseMetrics(agentsQuery);

    // ============= Process agents from tags =============
    const topAgents = (agentsData.data || [])
      .filter((agent: any) => agent.tags && agent.tags.length > 0)
      .slice(0, 5)
      .map((agent: any) => {
        const requests = parseInt(agent.count_count || "0");

        // Get the first tag or join multiple tags
        const rawTag = Array.isArray(agent.tags) ? agent.tags[0] : agent.tags;
        const agentName = formatAgentName(rawTag); // ✅ Format the name

        return {
          name: agentName, // Now shows "Genel Maksat" instead of "genel-maksat"
          requests,
          tokens: Math.round(parseFloat(agent.totalTokens_sum || "0")),
          cost: Math.round(parseFloat(agent.totalCost_sum || "0") * 100) / 100,
          successRate: 100,
          avgLatency: Math.round(parseFloat(agent.latency_avg || "0")),
        };
      });
    // ============= QUERY 5: Active Users =============
    const usersQuery = {
      view: "traces",
      metrics: [{ measure: "count", aggregation: "count" }],
      dimensions: [{ field: "userId" }],
      filters: [],
      fromTimestamp,
      toTimestamp,
    };

    let activeUsers = 0;
    try {
      const usersData = await callLangFuseMetrics(usersQuery);
      activeUsers = usersData.data?.filter((u: any) => u.userId && u.userId !== null).length || 0;
    } catch (err) {
      console.warn("Could not fetch users:", err);
    }

    // ============= QUERY 6: Recent Error Observations =============
    const recentErrorsQuery = {
      view: "observations",
      metrics: [{ measure: "count", aggregation: "count" }],
      dimensions: [{ field: "name" }, { field: "traceName" }],
      filters: [{ column: "level", operator: "=", value: "ERROR", type: "string" }],
      fromTimestamp,
      toTimestamp,
      config: {
        row_limit: 5,
      },
    };

    let recentErrors: any[] = [];
    try {
      const recentErrorsData = await callLangFuseMetrics(recentErrorsQuery);
      recentErrors = (recentErrorsData.data || []).slice(0, 5).map((error: any, idx: number) => ({
        id: `error-${idx}`,
        timestamp: new Date().toISOString(),
        agent: error.traceName || error.name || "Unknown",
        error: error.name || "Error occurred",
      }));
    } catch (err) {
      console.warn("Could not fetch recent errors:", err);
    }

    // ============= PROCESS RESULTS =============

    const totalTraces = parseInt(overall.count_count || "0");
    const totalTokens = parseFloat(overall.totalTokens_sum || "0");
    const totalCost = parseFloat(overall.totalCost_sum || "0");
    const avgLatency = Math.round(parseFloat(overall.latency_avg || "0"));

    const successRate = totalTraces > 0 ? Math.round(((totalTraces - errorCount) / totalTraces) * 1000) / 10 : 100;

    const metrics = {
      // Overview
      totalTraces,
      totalTokens: Math.round(totalTokens),
      totalCost: Math.round(totalCost * 100) / 100,
      avgLatency,
      successRate,
      errorRate: Math.round((100 - successRate) * 10) / 10,

      // Users
      activeUsers,
      totalSessions: totalTraces,

      // Performance
      latencyPercentiles: {
        p50: Math.round(parseFloat(latency.latency_p50 || "0")),
        p95: Math.round(parseFloat(latency.latency_p95 || "0")),
        p99: Math.round(parseFloat(latency.latency_p99 || "0")),
      },

      // Agents
      topAgents,

      // Errors
      recentErrors,
      totalErrors: errorCount,

      // Meta
      timeRange: range,
      lastUpdated: new Date().toISOString(),
    };

    console.log("✅ Final metrics:", metrics);

    return NextResponse.json(metrics);
  } catch (error: any) {
    console.error("❌ Error:", error);

    // Mock data fallback
    return NextResponse.json({
      totalTraces: 1250,
      totalTokens: 450000,
      totalCost: 15.5,
      avgLatency: 850,
      successRate: 98.5,
      errorRate: 1.5,
      activeUsers: 42,
      totalSessions: 156,
      latencyPercentiles: { p50: 650, p95: 1200, p99: 2500 },
      topAgents: [
        { name: "Customer Support", requests: 520, tokens: 180000, cost: 6.2, successRate: 99.2, avgLatency: 720 },
        { name: "Data Analyzer", requests: 380, tokens: 145000, cost: 5.8, successRate: 97.4, avgLatency: 950 },
        { name: "Code Assistant", requests: 210, tokens: 95000, cost: 3.5, successRate: 98.1, avgLatency: 680 },
      ],
      recentErrors: [],
      totalErrors: 0,
      timeRange: "24h",
      lastUpdated: new Date().toISOString(),
      _mock: true,
      _error: error.message,
    });
  }
}
