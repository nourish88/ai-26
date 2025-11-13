import { fetchAllApplications } from "@/data-access/fetch-all-apps";
import { fetchConversationDetail } from "../_data-access/FetchConversationList";
import type { Message } from "../_types/message";
import ChatPage from "../client";
import { notFound } from "next/navigation";
import { fetchAllFiles } from "@/data-access/fetch-user-files";

async function Page({ params }: { params: Promise<{ threadId: string; identifier: string }> }) {
  const { threadId, identifier } = await params;
  let initialMessages: Message[] | undefined = undefined;
  console.log("Identifier from URL:", identifier); // Will print "test123"
  // 1. Fetch the list of all apps (this will be cached and fast)
  const allApps = await fetchAllApplications();
  const allFiles = await fetchAllFiles(identifier);
  console.log("allFiles", allFiles);
  // 2. ✅ FIND the specific app object that matches the identifier from the URL.
  // This is the key step. We use the array `.find()` method.
  const currentApp = allApps.find((app: { identifier: string }) => app.identifier === identifier);
  console.log("currentApp", currentApp);
  // 3. ✅ HANDLE the case where the identifier is invalid.
  if (!currentApp) {
    // This will stop rendering and show the nearest not-found.js file.
    notFound();
  }

  if (threadId !== undefined) {
    initialMessages = await fetchConversationDetail(threadId, identifier);
  }
  console.log("initialMessages", initialMessages);
  const id = threadId?.[0] ?? null;
  return <ChatPage allFiles={allFiles} initialMessages={initialMessages} threadId={id} identifier={identifier} app={currentApp} />;
}

export default Page;
