// hooks/MutationOptions.ts
import { type QueryClient } from "@tanstack/react-query";

import { createFireStore, deleteFileStore, updateFileStore } from "../_service/DataMutation";

import { llmKeys } from "./useLLMEmbedding";

/**
 * Returns the options object for the create LLM provider mutation.
 * @param queryClient - The QueryClient instance from useQueryClient()
 */
export const createFireStoresMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: createFireStore,
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
export const updateFireStoresMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: updateFileStore,
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
export const deleteFireStoresMutationOptions = (queryClient: QueryClient) => {
  return {
    mutationFn: deleteFileStore,
    onSuccess: () => {
      return queryClient.invalidateQueries({ queryKey: llmKeys.lists() });
    },
    onError: (error: any) => {
      debugger;
      return error;
    },
  };
};
