"use server";
import { api } from "@/app/api/api";

export const fetchChunkingStrageties = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("chunkingstrategies", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
