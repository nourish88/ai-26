// hooks/useLLMs.ts
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createFireStoresMutationOptions, deleteFireStoresMutationOptions, updateFireStoresMutationOptions } from "./MutationOptions";
import { fetchFileStoresQueryOptions } from "./QueryOptions";

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
  return useQuery(fetchFileStoresQueryOptions(pageIndex, pageSize));
}

/**
 * Create new LLM
 */
export function useCreateLLMEmbedding() {
  const queryClient = useQueryClient();
  return useMutation(createFireStoresMutationOptions(queryClient));
}

/**
 * Update existing LLM
 */
export function useUpdateLLMEmbedding() {
  const queryClient = useQueryClient();
  return useMutation(updateFireStoresMutationOptions(queryClient));
}

/**
 *  Delete existing LLM
 * @returns
 */
export function useDeleteLLMEmbedding() {
  const queryClient = useQueryClient();
  return useMutation(deleteFireStoresMutationOptions(queryClient));
}
