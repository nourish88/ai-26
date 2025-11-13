// hooks/MutationOptions.ts
import { type QueryClient } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { createAppSearchEngines, updateAppSearchEngines, deleteAppSearchEngines } from "../_service/DataMutation";

/**
 * Returns the options object for the create LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const createAppSearchEnginesMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: createAppSearchEngines,
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
export const updateAppSearchEnginesMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: updateAppSearchEngines,
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
export const deleteAppSearchEnginesMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: deleteAppSearchEngines,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      return error;
    },
  };
};
