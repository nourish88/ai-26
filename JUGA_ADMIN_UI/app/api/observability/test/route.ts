// app/api/observability/test/route.ts
import { NextResponse } from "next/server";

const LANGFUSE_BASE_URL = process.env.LANGFUSE_BASE_URL || "http://127.0.0.1:3050";
const LANGFUSE_PUBLIC_KEY = process.env.LANGFUSE_PUBLIC_KEY!;
const LANGFUSE_SECRET_KEY = process.env.LANGFUSE_SECRET_KEY!;

export async function GET() {
  try {
    console.log("🔍 Testing LangFuse connection...");
    console.log("Base URL:", LANGFUSE_BASE_URL);
    console.log("Public Key:", LANGFUSE_PUBLIC_KEY?.substring(0, 10) + "...");

    const response = await fetch(`${LANGFUSE_BASE_URL}/api/public/health`, {
      headers: {
        Authorization: `Basic ${Buffer.from(`${LANGFUSE_PUBLIC_KEY}:${LANGFUSE_SECRET_KEY}`).toString("base64")}`,
      },
    });

    if (!response.ok) {
      throw new Error(`Health check failed: ${response.statusText}`);
    }

    const data = await response.json();

    return NextResponse.json({
      success: true,
      message: "Successfully connected to LangFuse",
      baseUrl: LANGFUSE_BASE_URL,
      health: data,
    });
  } catch (error: any) {
    console.error("❌ LangFuse connection test failed:", error);
    return NextResponse.json(
      {
        success: false,
        error: error.message,
        baseUrl: LANGFUSE_BASE_URL,
        hint: "Make sure LangFuse is running on http://127.0.0.1:3050 and credentials are correct",
      },
      { status: 500 }
    );
  }
}
