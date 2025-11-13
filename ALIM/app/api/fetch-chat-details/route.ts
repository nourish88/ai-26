import { Message } from "@/app/apps/[identifier]/_types/message";
import { auth } from "@/auth";
import { type NextRequest, NextResponse } from "next/server";

export async function GET(request: NextRequest) {
  try {
    const searchParams = request.nextUrl.searchParams;
    const threadId = searchParams.get("thread_id");

    if (!threadId) {
      return NextResponse.json({ error: "thread_id is required" }, { status: 400 });
    }

    const session = await auth();
    const response = await fetch(`${process.env.AI_ORCH_URL}/chat_history_detail?thread_id=${threadId}`, {
      headers: {
        "app-identifier": "test-app",
        Authorization: `Bearer ${session?.accessToken}`,
      },
      cache: "no-store",
    });

    if (!response.ok) {
      return NextResponse.json({ error: "Failed to fetch chat details" }, { status: response.status });
    }

    const historyItems: any[] = await response.json();

    const messages: Message[] = historyItems.flatMap((item) => {
      const userMessage: Message = {
        id: item.message_id,
        role: "user",
        content: item.query,
        timestamp: new Date(item.query_time_stamp),
        thread_id: item.thread_id,
      };

      const assistantMessage: Message = {
        id: item.message_id,
        role: "assistant",
        content: item.response,
        timestamp: new Date(item.response_time_stamp),
        thread_id: item.thread_id,
      };

      return [userMessage, assistantMessage];
    });

    messages.sort((a, b) => a.timestamp.getTime() - b.timestamp.getTime());

    return NextResponse.json(messages);
  } catch (error) {
    console.error("Fetch chat details error:", error);
    return NextResponse.json({ error: "Internal server error" }, { status: 500 });
  }
}
