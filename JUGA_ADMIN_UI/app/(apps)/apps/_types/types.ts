export interface AppChunkingStrategies {
  id: number;
  name: string;
  identifier: string;
  hasApplicationFile: boolean;
  hasUserFile: boolean;
  description: string;
  systemPrompt: string;
  applicationTypeId: number;
  memoryTypeId: number;
  outputTypeId: number;
}

export interface PaginatedLLMEmbeddingModelResponse {
  items: AppChunkingStrategies[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
