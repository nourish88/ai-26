"use server";
import { api } from "@/app/api/api";

export const fetchDataManagerExtractionEngineTypes = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("applicationextractorengine", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
