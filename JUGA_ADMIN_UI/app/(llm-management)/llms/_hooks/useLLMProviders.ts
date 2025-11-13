// hooks/useLLMs.ts
import { api } from "@/app/api/api";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { fetchLlmModelsQueryOptions } from "./QueryOptions";

import { createLlmModelMutationOptions, deleteLlmProviderMutationOptions, updateLlmProviderMutationOptions } from "./MutationOptions";

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
export function useLLMModels(pageIndex = 0, pageSize = 10) {
  return useQuery(fetchLlmModelsQueryOptions(pageIndex, pageSize));
}

/**
 * Create new LLM
 */
export function useCreateLLMModel() {
  const queryClient = useQueryClient();
  return useMutation(createLlmModelMutationOptions(queryClient));
}

/**
 * Update existing LLM
 */
export function useUpdateLLMModel() {
  const queryClient = useQueryClient();
  return useMutation(updateLlmProviderMutationOptions(queryClient));
}

/**
 *  Delete existing LLM
 * @returns
 */
export function useDeleteLLMModel() {
  const queryClient = useQueryClient();
  return useMutation(deleteLlmProviderMutationOptions(queryClient));
}
