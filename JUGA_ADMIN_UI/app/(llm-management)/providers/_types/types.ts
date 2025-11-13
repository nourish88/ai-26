export interface LLMProvider {
  id?: number;
  name: string;
}

export interface PaginatedLLMProviderResponse {
  items: LLMProvider[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}
