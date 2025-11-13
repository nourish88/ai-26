// Wizard step types
export type WizardStep = "app" | "llm" | "file-store" | "search-engines" | "embeddings" | "chunking" | "extractor" | "index";

export interface WizardStepConfig {
  id: WizardStep;
  title: string;
  description: string;
  optional?: boolean;
}

// Application types
export interface Application {
  id?: number;
  name: string;
  description?: string;
}

export interface LLMModel {
  id?: number;
  modelName: string;
  llmProviderId: number;
  maxInputTokenSize: number;
  maxOutputTokenSize: number;
  url: string;
}

export interface EmbeddingModel {
  id?: number;
  modelName: string;
  llmProviderId: number;
  maxInputTokenSize: number;
  vectorSize: number;
  url: string;
}

export interface FileStore {
  id?: number;
  applicationId: number;
  fileStoreId: number;
}

export interface AppSearchEngines {
  applicationId: number;
  searchEngineId: number;
  indexName?: string;
  embeddingId?: number;
  identifier?: string;
}

export interface AppChunkingStrategies {
  id?: number;
  applicationId: number;
  chunkingStrategyId: number;
}

export interface DataManagerExtractionEngineType {
  id?: number;
  applicationId: number;
  extractorEngineTypeId: number;
}

export interface WizardState {
  currentStep: WizardStep;
  completedSteps: WizardStep[];
  application?: Application;
  llmModel?: LLMModel;
  embedding?: EmbeddingModel;
  fileStore?: FileStore;
  searchEngine?: AppSearchEngines;
  chunkingStrategy?: AppChunkingStrategies;
  extractorEngine?: DataManagerExtractionEngineType;
}
