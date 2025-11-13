import { api } from "@/app/api/api";
import { queryOptions, useQuery } from "@tanstack/react-query";
import { llmKeys } from "./useLLMProviders";
import { fetchLlmProviders } from "../_service/DataAccess";
import { createLlmProvider, updateLlmProvider } from "../_service/DataMutation";
import { LLMProvider } from "../_types/types";

export const fetchLlmProvidersQueryOptions = (pageIndex = 0, pageSize = 10) => {
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchLlmProviders(pageIndex, pageSize),
    enabled: true,
  });
};

export const createLlmProviderQueryOptions = (llmData: LLMProvider) => {
  return queryOptions({
    queryKey: llmKeys.list(),
    queryFn: () => createLlmProvider(llmData),
    enabled: true,
  });
};

export const updateLlmProviderQueryOptions = (llmData: LLMProvider) => {
  return queryOptions({
    queryKey: llmKeys.list(),
    queryFn: () => updateLlmProvider(llmData),
    enabled: true,
  });
};
