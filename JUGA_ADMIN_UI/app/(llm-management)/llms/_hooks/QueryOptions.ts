import { queryOptions } from "@tanstack/react-query";
import { fetchLlmModels } from "../_service/DataAccess";
import { llmKeys } from "./useLLMProviders";

export const fetchLlmModelsQueryOptions = (pageIndex = 0, pageSize = 10) => {
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchLlmModels(pageIndex, pageSize),
    enabled: true,
  });
};
