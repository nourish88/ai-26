export interface DataManagerExtractionEngineType {
  id?: number;
  applicationId?: number;
  extractorEngineTypeId?: number;
}

export interface PaginatedLLMEmbeddingModelResponse {
  items: DataManagerExtractionEngineType[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
