"use server";
import { api } from "@/app/api/api";

export const fetchAppSearchEngines = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("applicationsearchengines", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
