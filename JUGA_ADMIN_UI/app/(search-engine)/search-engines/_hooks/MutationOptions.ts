// hooks/MutationOptions.ts
import { type QueryClient } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { createSearchEngines, updateSearchEngines, deleteSearchEngines } from "../_service/DataMutation";

/**
 * Returns the options object for the create LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const createSearchEnginesMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: createSearchEngines,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      return error;
    },
  };
};

/**
 * Returns the options object for the update LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const updateSearchEnginesMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: updateSearchEngines,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      return error;
    },
  };
};

/**
 * Returns the options object for the delete LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const deleteSearchEnginesMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: deleteSearchEngines,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      return error;
    },
  };
};
