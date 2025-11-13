"use client";

import { ChatHeader } from "./_components/ChatHeader";
import { ChatInputWithUpload } from "./_components/ChatInput";
import { MessageList } from "./_components/ChatMessages";

import { useChat } from "./_hooks/useChat";
import type { AppDefinition, Message } from "./_types/message";

interface ChatPageProps {
  initialMessages?: Message[];
  threadId?: string | null;
  identifier: string;
  app?: AppDefinition;
  allFiles?: any[];
}

export default function ChatPage({ initialMessages, threadId, identifier, app, allFiles }: ChatPageProps) {
  const { messages, input, isLoading, isSidebarOpen, scrollAreaRef, inputRef, isFirstMessage, canSendMessage, setInput, handleSendMessage, handleKeyDown, copyMessage, toggleSidebar, exportToPDF, setSelectedFileIds } = useChat({
    initialMessages,
    threadId: threadId || undefined,
    identifier: identifier || undefined,
  });

  return (
    <main className="flex-1 flex flex-col bg-transparent backdrop-blur-lg">
      <ChatHeader identifier={identifier} appName={app?.name} isSidebarOpen={isSidebarOpen} onToggleSidebar={toggleSidebar} hasMessages={!isFirstMessage} onExportPDF={exportToPDF} />
      <div className="flex-1 overflow-hidden">
        <MessageList identifier={identifier} isLoading={isLoading} ref={scrollAreaRef} messages={messages} isFirstMessage={isFirstMessage} onSendMessage={handleSendMessage} onCopyMessage={copyMessage} />
      </div>
      <ChatInputWithUpload
        identifier={identifier}
        hasUserFile={app?.hasUserFile}
        ref={inputRef}
        input={input}
        isLoading={isLoading}
        canSendMessage={canSendMessage}
        onInputChange={(e) => setInput(e.target.value)}
        onSendMessage={() => handleSendMessage(input)}
        onKeyDown={handleKeyDown}
        allFiles={allFiles}
        onSelectionChange={setSelectedFileIds}
      />
    </main>
  );
}
