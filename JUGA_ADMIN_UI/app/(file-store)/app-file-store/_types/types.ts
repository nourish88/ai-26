export interface FileStore {
  id?: number;
  applicationId: number | string;
  fileStoreId: number | string;
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
