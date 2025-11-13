// Enum types from API
export enum ApplicationTypes {
  Type1 = 1,
  Type2 = 2,
  Type3 = 3,
  Type4 = 4,
  Type99 = 99,
}

export enum MemoryTypes {
  Type1 = 1,
  Type2 = 2,
  Type3 = 3,
}

export enum OutputTypes {
  Type1 = 1,
  Type2 = 2,
  Type3 = 3,
}

export enum FileTypes {
  Type1 = 1,
  Type2 = 2,
}

export enum IngestionStatusTypes {
  Type1 = 1,
  Type2 = 2,
  Type3 = 3,
  Type4 = 4,
  Type5 = 5,
  Type6 = 6,
  Type7 = 7,
}

// API Request/Response types
export interface CreateApplicationCommand {
  name: string;
  identifier: string;
  hasApplicationFile: boolean;
  hasUserFile: boolean;
  enableGuardRails: boolean;
  checkHallucination: boolean;
  description?: string;
  systemPrompt?: string;
  applicationTypeId: ApplicationTypes;
  memoryTypeId: MemoryTypes;
  outputTypeId: OutputTypes;
}

export interface ApplicationDto {
  id: number;
  name: string;
  identifier: string;
  hasApplicationFile: boolean;
  hasUserFile: boolean;
  enableGuardRails: boolean;
  checkHallucination: boolean;
  description?: string;
  systemPrompt?: string;
  applicationTypeId: ApplicationTypes;
  memoryTypeId: MemoryTypes;
  outputTypeId: OutputTypes;
}

export interface CreateApplicationLlmCommand {
  topP: number;
  temperature: number;
  enableThinking: boolean;
  llmId: number;
  applicationId: number;
}

export interface ApplicationLlmDto {
  id: number;
  topP: number;
  temperature: number;
  enableThinking: boolean;
  applicationId: number;
  llmId: number;
}

export interface LlmDto {
  id: number;
  llmProviderId: number;
  maxInputTokenSize: number;
  maxOutputTokenSize: number;
  url: string;
  modelName: string;
}

export interface CreateApplicationFileStoreCommand {
  applicationId: number;
  fileStoreId: number;
}

export interface ApplicationFileStoreDto {
  id: number;
  applicationId: number;
  fileStoreId: number;
}

export interface FileStoreDto {
  id: number;
  identifier: string;
  uri: string;
}

export interface CreateApplicationSearchEngineCommand {
  applicationId: number;
  searchEngineId: number;
  indexName: string;
  embeddingId: number;
  identifier: string;
}

export interface ApplicationSearchEngineDto {
  id: number;
  applicationId: number;
  searchEngineId: number;
  indexName: string;
  embeddingId: number;
  identifier: string;
}

export interface SearchEngineDto {
  id: number;
  searchEngineTypeId: number;
  identifier: string;
  url: string;
}

export interface CreateApplicationEmbeddingCommand {
  applicationId: number;
  embeddingId: number;
}

export interface ApplicationEmbeddingDto {
  id: number;
  applicationId: number;
  embeddingId: number;
}

export interface EmbeddingDto {
  id: number;
  llmProviderId: number;
  url: string;
  modelName: string;
  vectorSize: number;
  maxInputTokenSize: number;
}

export interface CreateApplicationChunkingStrategyCommand {
  applicationId: number;
  chunkingStrategyId: number;
  chunkSize?: number;
  overlap?: number;
  seperator?: string;
}

export interface ApplicationChunkingStrategyDto {
  id: number;
  applicationId: number;
  chunkingStrategyId: number;
  chunkSize?: number;
  overlap?: number;
  seperator?: string;
}

export interface ChunkingStrategyDto {
  id: number;
  identifier: string;
  isChunkingSizeRequired: boolean;
  isOverlapRequired: boolean;
}

export interface CreateApplicationExtractorEngineCommand {
  applicationId: number;
  extractorEngineTypeId: number;
}

export interface ApplicationExtractorEngineDto {
  id: number;
  applicationId: number;
  extractorEngineTypeId: number;
}

export interface ExtractorEngineTypeDto {
  id: number;
  identifier: string;
  word: boolean;
  txt: boolean;
  pdf: boolean;
}

// Paginated response type
export interface PageResponse<T> {
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
  items: T[];
}

// Generic API response wrappers
export interface QueryResult<T> {
  result: T;
}

export interface CommandResult<T> {
  result?: T;
  success?: boolean;
  message?: string;
  error?: string;
}
