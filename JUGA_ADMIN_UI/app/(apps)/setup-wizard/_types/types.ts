import { ApplicationLlmDto, ApplicationEmbeddingDto, ApplicationFileStoreDto, ApplicationSearchEngineDto, ApplicationChunkingStrategyDto, ApplicationExtractorEngineDto, ApplicationDto, LlmDto } from "./api-types";

export interface WizardStepConfig {
  id: WizardStep;
  title: string;
  description: string;
  optional?: boolean;
}

// Re-export API types for convenience
export type Application = ApplicationDto;
export type LLMModel = ApplicationLlmDto;
export type EmbeddingModel = ApplicationEmbeddingDto;
export type FileStore = ApplicationFileStoreDto;
export type AppSearchEngines = ApplicationSearchEngineDto;
export type AppChunkingStrategies = ApplicationChunkingStrategyDto;
export type DataManagerExtractionEngineType = ApplicationExtractorEngineDto;

// API Response types
export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
}

export type WizardStep = "app" | "model" | "data-integration" | "mcp" | "summary";

export type CompletedSteps = WizardStep[];

export type AgentType =
  | "CHATBOT" // Maps to CHATBOT
  | "REACT" // Maps to REACT
  | "AGENTIC_RAG" // Maps to AGENTIC_RAG (pure RAG)
  | "MCP_POWERED_AGENTIC_RAG"; // Maps to MCP_POWERED_AGENTIC_RAG

export interface ApplicationPayload {
  name: string;
  identifier: string;
  description: string;
  systemPrompt: string;
  applicationTypeId: number;
  memoryTypeId: number;
  outputTypeId: number;
  hasApplicationFile: boolean;
  hasUserFile: boolean;
  enableGuardRails: boolean;
  checkHallucination: boolean;
  agentType: AgentType;
}

export interface LlmPayload {
  llmId: number;
  topP: number;
  temperature: number;
  enableThinking: boolean;
}

export interface FileStoreState {
  fileStoreId: number;
  type?: string;
}

export interface SearchEngineState {
  searchEngineId: number;
  embeddingId: number;
  indexName: string;
  identifier?: string;
}

export interface ChunkingState {
  chunkingStrategyId: number;
  chunkSize: number;
  overlap: number;
  seperator?: string;
}

export interface ExtractorEngineState {
  extractorEngineTypeId: number;
}

// Changed from McpServersState to McpServerState (singular)
export interface McpServerState {
  mcpServerId: number;
}

export interface WizardData {
  application?: ApplicationPayload;
  llm?: LlmPayload;
  fileStore?: FileStoreState | null;
  searchEngine?: SearchEngineState | null;
  chunkingStrategy?: ChunkingState | null;
  extractorEngine?: ExtractorEngineState | null;
  mcpServer?: McpServerState | null; // Changed from mcpServers to mcpServer
}

export interface WizardState {
  currentStep: WizardStep;
  completedSteps: CompletedSteps;
  data: WizardData;
  activationComplete?: boolean;
  application?: { id: number; name: string; identifier: string } | null;
}
