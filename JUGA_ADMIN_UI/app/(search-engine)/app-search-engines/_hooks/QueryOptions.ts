import { queryOptions } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { fetchAppSearchEngines } from "../_service/DataAccess";

export const fetchAppChunkingStrategyQueryOptions = (pageIndex = 0, pageSize = 10) => {
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchAppSearchEngines(pageIndex, pageSize),
    enabled: true,
  });
};
