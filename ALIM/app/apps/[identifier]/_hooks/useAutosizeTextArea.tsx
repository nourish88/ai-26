"use client";

import { useEffect } from "react";

export function useAutosizeTextArea(textAreaRef: HTMLTextAreaElement | null, value: string) {
  useEffect(() => {
    if (textAreaRef) {
      // Reset height to auto to get the correct scrollHeight
      textAreaRef.style.height = "auto";

      // Set the height to match the scroll height
      const scrollHeight = textAreaRef.scrollHeight;
      textAreaRef.style.height = `${scrollHeight}px`;
    }
  }, [textAreaRef, value]);
}
