// app/chat/_hooks/useScrollToBottom.ts
"use client";

import { useCallback, useRef } from "react";

export function useScrollToBottom() {
  const scrollAreaRef = useRef<HTMLDivElement | null>(null);

  const scrollToBottom = useCallback(() => {
    console.log("scrollAreaRef", scrollAreaRef.current);
    if (!scrollAreaRef.current) return;

    const viewport = scrollAreaRef.current.querySelector("[data-radix-scroll-area-viewport]") as HTMLElement | null;

    if (viewport) {
      console.log("viewport", viewport);
      // Use scrollTo with smooth behavior for a better UX
      viewport.scrollTo({ top: viewport.scrollHeight, behavior: "smooth" });
    }
  }, []);

  return { scrollAreaRef, scrollToBottom };
}
