// hooks/MutationOptions.ts
import { type QueryClient } from "@tanstack/react-query";

import { createLlmModel, deleteLlmModel, updateLlmModel } from "../_service/DataMutation";

import { llmKeys } from "./useLLMProviders";

/**
 * Returns the options object for the create LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const createLlmModelMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: createLlmModel,
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
export const updateLlmProviderMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: updateLlmModel,
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
export const deleteLlmProviderMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: deleteLlmModel,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      debugger;
      return error;
    },
  };
};
