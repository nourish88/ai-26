import { queryOptions } from "@tanstack/react-query";

import { llmKeys } from "./useLLMEmbedding";
import { fetchDataManagerExtractionEngineTypes } from "../_service/DataAccess";

export const fetchDataManagerExtractorEngineTypeQueryOptions = (pageIndex = 0, pageSize = 10) => {
  return queryOptions({
    queryKey: llmKeys.list(pageIndex, pageSize),
    queryFn: () => fetchDataManagerExtractionEngineTypes(pageIndex, pageSize),
    enabled: true,
  });
};
