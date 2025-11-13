// hooks/MutationOptions.ts
import { type QueryClient } from "@tanstack/react-query";

import { createLlmProvider, deleteLlmProvider, updateLlmProvider } from "../_service/DataMutation";
import type { LLMProvider } from "../_types/types";
import { llmKeys } from "./useLLMProviders";

/**
 * Returns the options object for the create LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const createLlmProviderMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: createLlmProvider,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      debugger;
      return error;
    },
  };
};

/**
 * Returns the options object for the update LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const updateLlmProviderMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: updateLlmProvider,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      debugger;
      return error;
    },
  };
};

/**
 * Returns the options object for the delete LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const deleteLlmProviderMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: deleteLlmProvider,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      debugger;
      return error;
    },
  };
};
