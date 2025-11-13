export interface ExtractionEngineType {
  id?: number;

  identifier: string;
  word: boolean;
  txt: boolean;
  pdf: boolean;
}

export interface PaginatedLLMEmbeddingModelResponse {
  items: ExtractionEngineType[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
