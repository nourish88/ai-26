// hooks/MutationOptions.ts
import { type QueryClient } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { createDataManagerExtractorEngineType, deleteDataManagerExtractorEngineType, updateDataManagerExtractorEngineType } from "../_service/DataMutation";

/**
 * Returns the options object for the create LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const createDataManagerExtractorEngineTypeMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: createDataManagerExtractorEngineType,
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
export const updateDataManagerExtractorEngineTypeMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: updateDataManagerExtractorEngineType,
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
export const deleteDataManagerExtractorEngineTypeMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: deleteDataManagerExtractorEngineType,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      return error;
    },
  };
};
