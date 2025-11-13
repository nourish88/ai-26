import { queryOptions } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { fetchSearchEngines } from "../_service/DataAccess";

export const fetchAppChunkingStrategyQueryOptions = (pageIndex = 0, pageSize = 10) => {
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchSearchEngines(pageIndex, pageSize),
    enabled: true,
  });
};
