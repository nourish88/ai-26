import { cn } from "@/lib/utils";
import { Bot } from "lucide-react";
import { Avatar, AvatarFallback } from "./ui/avatar";

/**
 * A skeleton component to indicate that the assistant is typing.
 */
export const MessageSkeleton = () => {
  return (
    <div className="flex gap-4 animate-in fade-in duration-300">
      <Avatar className="w-9 h-9 mt-1 flex-shrink-0">
        <AvatarFallback className="bg-primary text-primary-foreground">
          <Bot className="w-5 h-5" />
        </AvatarFallback>
      </Avatar>
      <div className={cn("max-w-[85%] sm:max-w-[75%] space-y-2")}>
        <div className={cn("p-3 rounded-2xl text-sm leading-relaxed whitespace-pre-wrap shadow-lg bg-zinc-900/80 backdrop-blur-sm rounded-bl-none border border-white/10")}>
          <div className="flex items-center space-x-1">
            <span className="inline-block w-2 h-4 bg-current animate-pulse rounded-full" />
            <span className="inline-block w-2 h-4 bg-current animate-pulse rounded-full" style={{ animationDelay: "0.2s" }} />
            <span className="inline-block w-2 h-4 bg-current animate-pulse rounded-full" style={{ animationDelay: "0.4s" }} />
          </div>
        </div>
      </div>
    </div>
  );
};
