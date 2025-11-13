import { queryOptions } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { fetchAppChunkingStrageties } from "../_service/DataAccess";

export const fetchAppChunkingStrategyQueryOptions = (pageIndex = 0, pageSize = 10) => {
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchAppChunkingStrageties(pageIndex, pageSize),
    enabled: true,
  });
};
