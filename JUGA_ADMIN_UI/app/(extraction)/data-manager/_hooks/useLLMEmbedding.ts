// hooks/useLLMs.ts
import { api } from "@/app/api/api";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { fetchDataManagerExtractorEngineTypeQueryOptions } from "./QueryOptions";
import { createDataManagerExtractorEngineTypeMutationOptions, deleteDataManagerExtractorEngineTypeMutationOptions, updateDataManagerExtractorEngineTypeMutationOptions } from "./MutationOptions";

/**
 * Query keys for consistent cache management
 */
export const llmKeys = {
  all: ["llmmodel"] as const,
  lists: () => [...llmKeys.all, "list"] as const,
  list: (pageIndex?: number, pageSize?: number) => [...llmKeys.lists(), { pageIndex, pageSize }] as const,
};

/**
 * Fetch paginated LLMs
 */
export function useLLMEmbedding(pageIndex = 0, pageSize = 10) {
  return useQuery(fetchDataManagerExtractorEngineTypeQueryOptions(pageIndex, pageSize));
}

/**
 * Create new LLM
 */
export function useCreateLLMEmbedding() {
  const queryClient = useQueryClient();
  return useMutation(createDataManagerExtractorEngineTypeMutationOptions(queryClient));
}

/**
 * Update existing LLM
 */
export function useUpdateLLMEmbedding() {
  const queryClient = useQueryClient();
  return useMutation(updateDataManagerExtractorEngineTypeMutationOptions(queryClient));
}

/**
 *  Delete existing LLM
 * @returns
 */
export function useDeleteLLMEmbedding() {
  const queryClient = useQueryClient();
  return useMutation(deleteDataManagerExtractorEngineTypeMutationOptions(queryClient));
}
