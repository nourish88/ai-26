import { queryOptions } from "@tanstack/react-query";

import { fetchFileStores } from "../_service/DataAccess";
import { llmKeys } from "./useLLMEmbedding";

export const fetchFileStoresQueryOptions = (pageIndex = 0, pageSize = 10) => {
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchFileStores(pageIndex, pageSize),
    enabled: true,
  });
};
