"use server";
import { api } from "@/app/api/api";

export const fetchLlmModels = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("llms", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
