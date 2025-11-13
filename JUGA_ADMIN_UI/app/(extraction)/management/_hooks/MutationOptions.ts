// hooks/MutationOptions.ts
import { type QueryClient } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { createExtractorEngineType, deleteExtractorEngineType, updateExtractorEngineType } from "../_service/DataMutation";

/**
 * Returns the options object for the create LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const createExtractorEngineTypeMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: createExtractorEngineType,
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
export const updateExtractorEngineTypeMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: updateExtractorEngineType,
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
export const deleteExtractorEngineTypeMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: deleteExtractorEngineType,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      return error;
    },
  };
};
