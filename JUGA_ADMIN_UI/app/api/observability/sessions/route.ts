// app/api/observability/sessions/route.ts
import { NextRequest, NextResponse } from "next/server";

const LANGFUSE_BASE_URL = process.env.LANGFUSE_BASE_URL || "http://127.0.0.1:3050";
const LANGFUSE_PUBLIC_KEY = process.env.LANGFUSE_PUBLIC_KEY!;
const LANGFUSE_SECRET_KEY = process.env.LANGFUSE_SECRET_KEY!;

async function callLangFuseAPI(endpoint: string, options?: RequestInit) {
  const url = `${LANGFUSE_BASE_URL}/api/public${endpoint}`;

  const response = await fetch(url, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      Authorization: `Basic ${Buffer.from(`${LANGFUSE_PUBLIC_KEY}:${LANGFUSE_SECRET_KEY}`).toString("base64")}`,
      ...options?.headers,
    },
  });

  if (!response.ok) {
    throw new Error(`LangFuse API error: ${response.statusText}`);
  }

  return response.json();
}

export async function GET(request: NextRequest) {
  try {
    const searchParams = request.nextUrl.searchParams;
    const page = parseInt(searchParams.get("page") || "1");
    const limit = parseInt(searchParams.get("limit") || "50");

    const params = new URLSearchParams({
      page: page.toString(),
      limit: limit.toString(),
    });

    const response = await callLangFuseAPI(`/sessions?${params.toString()}`);

    return NextResponse.json(response);
  } catch (error: any) {
    console.error("Error fetching sessions:", error);
    return NextResponse.json({ error: "Failed to fetch sessions", details: error.message }, { status: 500 });
  }
}
