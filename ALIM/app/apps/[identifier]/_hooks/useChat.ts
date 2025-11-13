"use client";

import type React from "react";
import { useState, useRef, useEffect, useCallback, useMemo, useTransition } from "react";
import { jsPDF } from "jspdf";
import type { Message } from "../_types/message";
import { useRouter, useSearchParams } from "next/navigation";
import { useSession } from "next-auth/react";
import { v4 as uuidv4 } from "uuid";
import { revalidateHistory } from "../_actions/chatActions";

type UploadStatus = "idle" | "uploading" | "error";

const INITIAL_WELCOME_MESSAGE: Message = {
  id: "initial-welcome",
  content: "Merhaba! Ben yapay zeka asistanınız. Bugün size nasıl yardımcı olabilirim?",
  role: "assistant",
  timestamp: new Date(),
};

interface UseChatProps {
  initialMessages?: Message[];
  threadId?: string;
  identifier?: string;
}

export function useChat({ initialMessages, threadId: initialThreadId, identifier }: UseChatProps) {
  const [messages, setMessages] = useState<Message[]>(initialMessages && initialMessages.length > 0 ? initialMessages : [INITIAL_WELCOME_MESSAGE]);
  const [input, setInput] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [isSidebarOpen, setIsSidebarOpen] = useState(true);
  const router = useRouter();
  const scrollAreaRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLTextAreaElement>(null);
  const { data: session } = useSession();
  const [file, setFile] = useState<File | null>(null);
  const [uploadedDocumentId, setUploadedDocumentId] = useState<string | null>(null);
  const [uploadStatus, setUploadStatus] = useState<UploadStatus>("idle");
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [isUploading, startUploadTransition] = useTransition();
  const [isRedirecting, startRedirectTransition] = useTransition();
  const [selectedFileIds, setSelectedFileIds] = useState<string[]>([]);

  const [currentThreadId, setCurrentThreadId] = useState<string | undefined>(initialThreadId);

  useEffect(() => {
    setCurrentThreadId(initialThreadId);
  }, [initialThreadId]);

  useEffect(() => {
    // If we have initialMessages from the server and a threadId, replace local messages
    if (initialMessages && initialMessages.length > 0 && initialThreadId) {
      console.log("[v0] Syncing messages from server for thread:", initialThreadId);
      setMessages(initialMessages);
    }
  }, [initialMessages, initialThreadId]);

  const searchParams = useSearchParams();
  const newChat = searchParams.get("new");

  useEffect(() => {
    if (newChat === "1") {
      setMessages([INITIAL_WELCOME_MESSAGE]);
      setInput("");
      handleRemoveFile();
      setIsLoading(false);
      setFile(null);
      setUploadedDocumentId(null);
      setUploadStatus("idle");
      setCurrentThreadId(undefined);
    }
  }, [newChat]);

  const isFirstMessage = useMemo(() => {
    const realMessages = messages.filter((msg) => msg.id !== "initial-welcome");
    return realMessages.length === 0;
  }, [messages]);

  useEffect(() => {
    inputRef.current?.focus();
  }, []);

  const scrollToBottom = useCallback(() => {
    if (scrollAreaRef.current) {
      const scrollContainer = scrollAreaRef.current.querySelector("[data-radix-scroll-area-viewport]");
      if (scrollContainer) {
        scrollContainer.scrollTop = scrollContainer.scrollHeight;
      }
    }
  }, []);

  useEffect(() => {
    scrollToBottom();
  }, [messages, scrollToBottom]);

  const handleRemoveFile = useCallback(() => {
    setFile(null);
    setUploadedDocumentId(null);
    setUploadStatus("idle");
    setUploadError(null);
  }, []);

  const handleSendMessage = useCallback(
    async (messageContent?: string) => {
      const content = (messageContent || input).trim();
      if (!content || isLoading || isRedirecting) return;

      const isNewChat = !currentThreadId;

      const userMessage: Message = { id: Date.now().toString(), content, role: "user", timestamp: new Date() };
      setMessages((prevMessages) => {
        const messagesWithoutWelcome = prevMessages.filter((msg) => msg.id !== "initial-welcome");
        return [...messagesWithoutWelcome, userMessage];
      });

      if (!messageContent) {
        setInput("");
      }
      setIsLoading(true);

      const assistantMessageId = (Date.now() + 1).toString();
      const assistantMessage: Message = {
        id: assistantMessageId,
        content: "",
        role: "assistant",
        timestamp: new Date(),
        isStreaming: true,
      };
      setMessages((prev) => [...prev, assistantMessage]);

      const serverMessageId = uuidv4();
      const requestBody = {
        id: "string",
        message_id: serverMessageId,
        app_id: "1",
        thread_id: currentThreadId || uuidv4(),
        user_id: session?.user?.name || "jugaai",
        query: content,
        response: "string",
        is_liked: false,
        is_disliked: false,
        context: {},
        application_identifier: identifier,
        query_time_stamp: new Date().toISOString(),
        response_time_stamp: new Date().toISOString(),
        fileIdentifiers: selectedFileIds.map((id) => String(id)),
      };

      let newThreadId: string | null = null;
      let actualMessageId: string | null = null;

      try {
        const response = await fetch(`/api/stream/${identifier}`, {
          method: "POST",
          headers: { "Content-Type": "application/json", Authorization: `Bearer ${session?.accessToken}` },
          body: JSON.stringify(requestBody),
        });
        console.log("response", response);
        if (!response.ok || !response.body) throw new Error(`HTTP error! status: ${response.status}`);
        setSelectedFileIds([]);
        const reader = response.body.getReader();
        const decoder = new TextDecoder();
        let buffer = "";
        let isFirstChunk = true;
        console.log("buffer", buffer);
        while (true) {
          const { value, done } = await reader.read();
          if (done) break;

          buffer += decoder.decode(value, { stream: true });

          // Split buffer by newlines or }{  to separate JSON objects
          const jsonObjects = buffer.split(/\n|(?<=\})(?=\{)/);

          // Keep the last incomplete chunk in buffer
          buffer = jsonObjects.pop() || "";

          for (const jsonObjStr of jsonObjects) {
            const trimmed = jsonObjStr.trim();
            if (!trimmed) continue;

            try {
              const parsed = JSON.parse(trimmed);

              // Handle thread_id for new chats
              if (isNewChat && isFirstChunk && parsed.thread_id) {
                newThreadId = parsed.thread_id;
                setCurrentThreadId(newThreadId as any);
              }

              // Handle message_id
              if (parsed.message_id && !actualMessageId) {
                actualMessageId = parsed.message_id;
                console.log("[v0] Received message_id:", actualMessageId);
              }

              // Handle response token - APPEND instead of replace
              const textChunk = parsed.response;
              if (textChunk) {
                if (isFirstChunk) {
                  setIsLoading(false);
                  isFirstChunk = false;
                }

                // ✅ APPEND each token to build the complete message
                setMessages((prev) => prev.map((msg) => (msg.id === assistantMessageId ? { ...msg, content: msg.content + textChunk, isStreaming: true } : msg)));
              }
            } catch (error) {
              console.error("Failed to parse JSON chunk:", error);
              // Put the chunk back in buffer for next iteration
              buffer = trimmed + buffer;
            }
          }
        }

        // Handle any remaining data in buffer after stream ends
        if (buffer.trim()) {
          try {
            const parsed = JSON.parse(buffer.trim());
            if (parsed.response) {
              setMessages((prev) => prev.map((msg) => (msg.id === assistantMessageId ? { ...msg, content: msg.content + parsed.response, isStreaming: true } : msg)));
            }
          } catch (error) {
            console.error("Failed to parse final buffer:", error);
          }
        }

        const finalMessageId = actualMessageId || serverMessageId;
        console.log("[v0] Updating message IDs to:", finalMessageId);

        setMessages((prev) =>
          prev.map((msg) => {
            if (msg.id === assistantMessageId) {
              return { ...msg, id: finalMessageId, isStreaming: false };
            }
            // Also update the user message that was just sent
            if (msg.id === userMessage.id) {
              return { ...msg, id: finalMessageId };
            }
            return msg;
          })
        );

        if (isNewChat && newThreadId) {
          startRedirectTransition(() => {
            router.push(`/apps/${identifier}/${newThreadId}`);
          });
          await revalidateHistory(identifier || "");
        }
      } catch (error) {
        console.error("Error calling stream API:", error);
        const errorMessage = "Üzgünüm, bağlanmada sorun yaşıyorum. Lütfen daha sonra tekrar deneyin.";
        setIsLoading(false);
        setMessages((prev) => prev.map((msg) => (msg.id === assistantMessageId ? { ...msg, content: errorMessage, isStreaming: false } : msg)));
      } finally {
        inputRef.current?.focus();
      }
    },
    [input, isLoading, isRedirecting, currentThreadId, router, session?.accessToken, identifier, selectedFileIds, session?.user?.name]
  );

  const handleKeyDown = useCallback(
    (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
      if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault();
        handleSendMessage(input);
        setInput("");
      }
    },
    [handleSendMessage, input]
  );

  const copyMessage = useCallback((content: string) => {
    navigator.clipboard.writeText(content);
  }, []);

  const startNewChat = () => {
    startRedirectTransition(() => {
      router.push(`/apps/${identifier}?new=1`);
    });
  };

  const toggleSidebar = useCallback(() => setIsSidebarOpen((prev) => !prev), []);

  const exportToPDF = useCallback(() => {
    const pdf = new jsPDF();
    pdf.text("Sohbet Geçmişi", 10, 10);
    messages.forEach((msg, index) => {
      pdf.text(`${msg.role}: ${msg.content}`, 10, 20 + index * 10);
    });
    pdf.save("sohbet.pdf");
  }, [messages]);

  const canSendMessage = useMemo(() => (!!input.trim() || !!uploadedDocumentId) && !isLoading && !isUploading && !isRedirecting, [input, uploadedDocumentId, isLoading, isUploading, isRedirecting]);

  return {
    messages,
    input,
    isLoading,
    isSidebarOpen,
    scrollAreaRef,
    inputRef,
    isFirstMessage,
    canSendMessage,
    file,
    uploadStatus,
    uploadError,
    isUploading,
    uploadedDocumentId,
    setInput,
    handleSendMessage,
    handleKeyDown,
    isRedirecting,
    copyMessage,
    startNewChat,
    setIsSidebarOpen,
    toggleSidebar,
    exportToPDF,
    handleRemoveFile,
    setSelectedFileIds,
  };
}
