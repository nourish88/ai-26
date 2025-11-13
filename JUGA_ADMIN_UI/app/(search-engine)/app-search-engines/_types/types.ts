export interface AppSearchEngines {
  id?: number;
  applicationId: number;
  searchEngineId: number;
  indexName: string;
  embeddingId: number;
  identifier: string;
}

export interface PaginatedLLMEmbeddingModelResponse {
  items: AppSearchEngines[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
