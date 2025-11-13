import { forwardRef } from "react";
import { ScrollArea } from "@/components/ui/scroll-area";
import { MessageItem } from "@/components/message-item";
import { ChatEmptyState } from "./ChatEmptyState";
import type { Message } from "../_types/message";

interface MessageListProps {
  messages: Message[];
  isFirstMessage: boolean;
  onSendMessage: (message: string) => void;
  onCopyMessage: (content: string) => void;
  isLoading?: boolean;
  identifier: string;
}

export const MessageList = forwardRef<HTMLDivElement, MessageListProps>(({ identifier, isLoading, messages, isFirstMessage, onSendMessage, onCopyMessage }, ref) => {
  console.log("messages", messages);
  return (
    <ScrollArea ref={ref} className="h-full">
      <div className="p-4 sm:p-6 space-y-8 mx-auto">
        {isFirstMessage ? (
          <ChatEmptyState onSendMessage={onSendMessage} />
        ) : (
          <>
            {messages.map((message, idx) => (
              <MessageItem identifier={identifier} key={idx} message={message} onCopyMessage={onCopyMessage} />
            ))}
          </>
        )}
      </div>
    </ScrollArea>
  );
});

MessageList.displayName = "MessageList";
