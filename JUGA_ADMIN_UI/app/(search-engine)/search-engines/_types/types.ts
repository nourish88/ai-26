export interface SearchEngines {
  id?: number;
  searchEngineTypeId: number;
  identifier: string;
  url: string;
}

export interface PaginatedLLMEmbeddingModelResponse {
  items: SearchEngines[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
