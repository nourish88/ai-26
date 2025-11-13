import type React from "react";
import { Sidebar } from "./_components/ChatSidebar";
import { fetchConversationList } from "./_data-access/FetchConversationList";

export default async function ChatLayout({ children, params }: { children: React.ReactNode; params: Promise<{ identifier: string }> }) {
  const { identifier } = await params;

  const initialConversations = await fetchConversationList(identifier);

  return (
    <div className="flex h-screen w-full bg-gray-900 text-white">
      <Sidebar initialConversations={initialConversations} identifier={identifier} />
      {children}
    </div>
  );
}
