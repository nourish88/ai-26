"use server";
import { api } from "@/app/api/api";

export const fetchLlmEmbeddings = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("extractorenginetypes", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
