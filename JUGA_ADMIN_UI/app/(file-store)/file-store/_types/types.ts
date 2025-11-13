export interface FileStore {
  id?: number;
  identifier: string;
  uri: string;
}

export interface PaginatedLLMEmbeddingModelResponse {
  items: FileStore[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
