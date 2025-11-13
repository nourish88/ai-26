"use server";
import { api } from "@/app/api/api";

export const fetchLlmEmbeddings = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("embeddings", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
