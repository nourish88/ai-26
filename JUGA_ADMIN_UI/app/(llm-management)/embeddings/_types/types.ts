export interface EmbeddingModel {
  id?: number;
  llmProviderId?: number;
  url: string;
  modelName: string;
  vectorSize: number;
  maxInputTokenSize: number;
}

export interface PaginatedLLMEmbeddingModelResponse {
  items: EmbeddingModel[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
