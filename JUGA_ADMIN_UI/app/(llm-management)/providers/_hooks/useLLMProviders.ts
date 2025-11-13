// hooks/useLLMs.ts
import { api } from "@/app/api/api";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import type { LLMProvider } from "../_types/types";
import { fetchLlmProvidersQueryOptions } from "./QueryOptions";
import { createLlmProvider } from "../_service/DataMutation";
import { createLlmProviderMutationOptions, deleteLlmProviderMutationOptions, updateLlmProviderMutationOptions } from "./MutationOptions";

/**
 * Query keys for consistent cache management
 */
export const llmKeys = {
  all: ["llmproviders"] as const,
  lists: () => [...llmKeys.all, "list"] as const,
  list: (pageIndex?: number, pageSize?: number) => [...llmKeys.lists(), { pageIndex, pageSize }] as const,
};

/**
 * Fetch paginated LLMs
 */
export function useLLMs(pageIndex = 0, pageSize = 10) {
  return useQuery(fetchLlmProvidersQueryOptions(pageIndex, pageSize));
}

/**
 * Create new LLM
 */
export function useCreateLLM() {
  const queryClient = useQueryClient();
  return useMutation(createLlmProviderMutationOptions(queryClient));
}

/**
 * Update existing LLM
 */
export function useUpdateLLM() {
  const queryClient = useQueryClient();
  return useMutation(updateLlmProviderMutationOptions(queryClient));
}

/**
 *  Delete existing LLM
 * @returns
 */
export function useDeleteLLM() {
  const queryClient = useQueryClient();
  return useMutation(deleteLlmProviderMutationOptions(queryClient));
}
