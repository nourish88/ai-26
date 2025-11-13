import { queryOptions } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { fetchChunkingStrageties } from "../_service/DataAccess";

export const fetchExtractorEngineTypeQueryOptions = (pageIndex = 0, pageSize = 10) => {
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchChunkingStrageties(pageIndex, pageSize),
    enabled: true,
  });
};
