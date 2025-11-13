"use server";
import { api } from "@/app/api/api";

export const fetchLlmProviders = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("llmproviders", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
