export interface Conversation {
  thread_id: string;
  title: string;
  created_at: string;
}

export interface Message {
  id: string;
  content: string;
  role: "user" | "assistant";
  timestamp: Date;
  isStreaming?: boolean;
  status?: "sending" | "sent" | "error";
  thread_id?: string;
}

export interface HistoryItem {
  message_id: string;
  thread_id: string;
  query: string;
  response: string;
  query_time_stamp: string;
  response_time_stamp: string;
}

export interface AppDefinition {
  name: string;
  identifier: string;
  hasApplicationFile: boolean;
  hasUserFile: boolean;
  description: string;
}
