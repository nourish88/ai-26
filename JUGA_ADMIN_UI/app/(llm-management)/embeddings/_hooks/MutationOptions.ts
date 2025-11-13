// hooks/MutationOptions.ts
import { type QueryClient } from "@tanstack/react-query";

import { createLlmModel, deleteLlmModel, updateLlmModel } from "../_service/DataMutation";

import { llmKeys } from "./useLLMEmbedding";

/**
 * Returns the options object for the create LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const createLLMEmdebdingModelMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: createLlmModel,
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
export const updateLLMEmbeddingModelMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: updateLlmModel,
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
export const deleteLLMEmbeddingModelMutationOptions = (queryClient: QueryClient) => {
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
