import { queryOptions } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { fetchAppChunkingStrageties } from "../_service/DataAccess";

export const fetchAppChunkingStrategyQueryOptions = (pageIndex: number, pageSize: number) => {
  console.log("pageIndex", pageIndex);
  console.log("pageSize", pageSize);
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchAppChunkingStrageties(pageIndex, pageSize),
    enabled: true,
  });
};
