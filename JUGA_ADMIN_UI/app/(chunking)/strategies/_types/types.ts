export interface ChunkingStrategies {
  id?: number;
  identifier: string;
  isChunkingSizeRequired: boolean;
  isOverlapRequired: boolean;
}

export interface PaginatedLLMEmbeddingModelResponse {
  items: ChunkingStrategies[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
