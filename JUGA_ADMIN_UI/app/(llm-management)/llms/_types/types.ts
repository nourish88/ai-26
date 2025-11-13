export interface LLMModel {
  llmProviderId?: number;
  id?: number;
  maxInputTokenSize: number;
  maxOutputTokenSize: number;
  url: string;
  modelName: string;
}

export interface PaginatedLLMModelResponse {
  items: LLMModel[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
