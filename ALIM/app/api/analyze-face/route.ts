import { auth } from "@/auth";
import { type NextRequest, NextResponse } from "next/server";

export async function POST(request: NextRequest) {
  try {
    const data = await auth();
    const formData = await request.formData();
    const file = formData.get("file") as File;

    if (!file) {
      return NextResponse.json({ error: "No file provided" }, { status: 400 });
    }

    // Validate file type
    if (!file.type.startsWith("image/")) {
      return NextResponse.json({ error: "File must be an image" }, { status: 400 });
    }

    // Validate file size (max 10MB)
    const maxSize = 10 * 1024 * 1024; // 10MB
    if (file.size > maxSize) {
      return NextResponse.json({ error: "File size must be less than 10MB" }, { status: 400 });
    }
    const externalFormData = new FormData();
    externalFormData.append("file", file);

    console.log(" Making request to external API with token:", data?.accessToken);
    console.log(" File details:", { name: file.name, type: file.type, size: file.size });
    const baseURL = "https://icaoanalyzer-test.jandarma.gov.tr/analyze-face/";
    console.log("Request Base URL", baseURL);
    const externalResponse = await fetch(baseURL, {
      method: "POST",
      headers: {
        Authorization: `Bearer ${data?.accessToken}`,
        accept: "application/json",
      },
      body: externalFormData,
    });

    console.log("External API response status:", externalResponse.status);

    if (!externalResponse.ok) {
      const errorText = await externalResponse.text();
      console.error("External API error:", errorText);
      return NextResponse.json({ error: `External API error: ${externalResponse.status} ${externalResponse.statusText}` }, { status: externalResponse.status });
    }

    const analysisResult = await externalResponse.json();
    console.log("[v0] Analysis result received:", analysisResult);
    return NextResponse.json(analysisResult);
  } catch (error) {
    console.error("[v0] Error processing file:", error);
    return NextResponse.json({ error: "Internal server error" }, { status: 500 });
  }
}
