export interface AppChunkingStrategies {
  id?: number;
  applicationId: number;
  chunkingStrategyId: number;
  chunkSize: number;
  overlap: number;
  seperator: any;
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
