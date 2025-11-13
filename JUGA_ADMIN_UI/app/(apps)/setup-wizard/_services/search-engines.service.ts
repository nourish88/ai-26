"use server";
import { api } from "@/app/api/api";
import { AppSearchEngines } from "../types";

export const createAppSearchEngines = async (searchEnginesData: Omit<AppSearchEngines, "id">) => {
  const response = await api.post("applicationsearchengines", searchEnginesData);
  return response.data;
};

export const updateAppSearchEngines = async (searchEnginesData: AppSearchEngines) => {
  const response = await api.put("applicationsearchengines", searchEnginesData);
  return response.data;
};

export const deleteAppSearchEngines = async (searchEnginesId: number) => {
  const response = await api.delete(`applicationsearchengines/${searchEnginesId}`);
  return response.data;
};

export const getSearchEngines = async () => {
  const response = await api.get("searchengines", {
    params: { pageIndex: 0, pageSize: 100 },
  });
  return response.data;
};
