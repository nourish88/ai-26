"use server";
import { api } from "@/app/api/api";

export const fetchSearchEngines = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("searchengines", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
