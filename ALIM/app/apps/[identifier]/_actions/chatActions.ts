"use server";

import { auth } from "@/auth";
import { revalidatePath, revalidateTag } from "next/cache";

export const revalidateHistory = async (identifier: string) => {
  revalidateTag(`conversations-${identifier}`);
};
export async function revalidateChatList(identifier: string) {
  revalidatePath(`/apps/${identifier}`, "layout");
}

export async function deleteChatAction(thread_id: string, identifier: string): Promise<{ success: boolean; message: string; redirectTo?: string }> {
  const session = await auth();

  if (!identifier) {
    console.error("Identifier is required.");
    return { success: false, message: "Server configuration error." };
  }

  try {
    const response = await fetch(`${process.env.AI_ORCH_URL}/chat_history_detail?thread_id=${thread_id}`, {
      method: "DELETE",
      headers: {
        "app-identifier": identifier,
        Authorization: `Bearer ${session?.accessToken}`,
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: "Failed to delete the chat." }));
      throw new Error(errorData.message);
    }

    revalidateTag("chat-history");

    return {
      success: true,
      message: "Chat deleted successfully!",
      redirectTo: `/apps/${identifier}?new=1`,
    };
  } catch (error) {
    console.error("Server Action Error:", error);
    const errorMessage = error instanceof Error ? error.message : "An unexpected error occurred.";
    return { success: false, message: errorMessage };
  }
}

export async function likeMessageAction(messageId: string, identifier: string, comment: string): Promise<{ success: boolean; message: string }> {
  const session = await auth();

  if (!identifier) {
    console.error("Identifier is required.");
    return { success: false, message: "Server configuration error." };
  }

  try {
    const response = await fetch(`${process.env.AI_ORCH_URL}/like_message?message_id=${messageId}&comment=${comment}`, {
      method: "POST",
      headers: {
        "app-identifier": identifier,
        Authorization: `Bearer ${session?.accessToken}`,
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: "Failed to like the message." }));
      throw new Error(errorData.detail);
    }

    return { success: true, message: "Message liked successfully!" };
  } catch (error) {
    console.error("Server Action Error:", error);
    const errorMessage = error instanceof Error ? error.message : "An unexpected error occurred.";
    return { success: false, message: errorMessage };
  }
}

export async function dislikeMessageAction(messageId: string, identifier: string, comment: string): Promise<{ success: boolean; message: string }> {
  const session = await auth();

  if (!identifier) {
    console.error("Identifier is required.");
    return { success: false, message: "Server configuration error." };
  }

  try {
    const response = await fetch(`${process.env.AI_ORCH_URL}/report_message?message_id=${messageId}&comment=${comment}`, {
      method: "POST",
      headers: {
        "app-identifier": identifier,
        Authorization: `Bearer ${session?.accessToken}`,
      },
    });

    if (!response.ok) {
      const errorData = await response.json().catch(() => ({ message: "Failed to dislike the chat." }));
      throw new Error(errorData.detail);
    }

    // revalidateTag("chat-history");

    return { success: true, message: "Message disliked successfully!" };
  } catch (error) {
    console.error("Server Action Error:", error);
    const errorMessage = error instanceof Error ? error.message : "An unexpected error occurred.";
    return { success: false, message: errorMessage };
  }
}
