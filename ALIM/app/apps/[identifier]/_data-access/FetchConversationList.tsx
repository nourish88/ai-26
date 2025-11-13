"use server";

import { auth } from "@/auth";
import type { Conversation, HistoryItem, Message } from "../_types/message";

export const fetchConversationList = async (identifier: string): Promise<Conversation[]> => {
  try {
    const session = await auth();
    const res = await fetch(`${process.env.AI_ORCH_URL}/chat_history`, {
      headers: {
        "app-identifier": identifier, // Use dynamic identifier instead of hardcoded "test-app"
        Authorization: `Bearer ${session?.accessToken}`,
      },
      next: {
        tags: ["chat-history"],
      },
    });
    console.log("resresres", res);
    if (!res.ok) {
      // console.error("Failed to fetch conversation list");
      return [];
    }

    const history: HistoryItem[] = await res.json();

    const conversationsMap = new Map<string, Conversation>();
    history.forEach((item) => {
      if (!conversationsMap.has(item.thread_id)) {
        conversationsMap.set(item.thread_id, {
          thread_id: item.thread_id,
          title: item.query,
          created_at: item.query_time_stamp,
        });
      }
    });

    const conversationList = Array.from(conversationsMap.values());
    conversationList.sort((a, b) => new Date(b.created_at).getTime() - new Date(a.created_at).getTime());
    return conversationList;
  } catch (error) {
    console.error("Error processing conversation list:", error);
    return [];
  }
};

export const fetchConversationDetail = async (threadId: string, identifier: string): Promise<Message[]> => {
  if (!threadId) throw new Error("Thread ID is required.");

  try {
    const session = await auth();
    const res = await fetch(`${process.env.AI_ORCH_URL}/chat_history_detail?thread_id=${threadId}`, {
      headers: {
        "app-identifier": identifier,
        Authorization: `Bearer ${session?.accessToken}`,
      },
      cache: "no-store",
    });

    if (!res.ok) {
      console.error("Failed to fetch conversation detail for thread:", threadId, res.statusText);
      return [];
    }

    const historyItems: any[] = await res.json();

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

    return messages;
  } catch (error) {
    console.error("Error processing conversation detail:", error);
    return [];
  }
};
