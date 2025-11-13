import { MessageSquare, Plus, Settings, Loader2 } from "lucide-react";

/**
 * This is the loading UI for the entire chat page route.
 * It mimics the final layout (a sidebar and a main content area).
 * Next.js will automatically show this file via Suspense when the data
 * for the page (from fetchConversationDetail in page.tsx) is being fetched on the server.
 */
export default function ChatLoading() {
  return (
    <div className="flex h-screen w-screen text-foreground">
      {/* 2. Fake Main Chat Area */}
      <main className="flex flex-1 flex-col">
        <div className="flex flex-1 items-center justify-center h-full">
          <div className="text-center text-muted-foreground">
            <Loader2 className="h-10 w-10 mx-auto mb-4 animate-spin" />
            <p className="font-medium text-lg">Sohbet Yükleniyor...</p>
            <p className="text-sm">Lütfen bekleyin.</p>
          </div>
        </div>
        <div className="p-4 border-t border-white/10">
          {/* Fake, disabled input */}
          <div className="w-full h-12 bg-black/10 rounded-md border border-white/10"></div>
        </div>
      </main>
    </div>
  );
}
