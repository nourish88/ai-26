import { queryOptions } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { fetchLlmEmbeddings } from "../_service/DataAccess";

export const fetchExtractorEngineTypeQueryOptions = (pageIndex = 0, pageSize = 10) => {
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchLlmEmbeddings(pageIndex, pageSize),
    enabled: true,
  });
};
