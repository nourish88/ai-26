import { api } from "@/app/api/api";
import { SearchEngines } from "../_types/types";

export const createSearchEngines = async (searchEnginesModelData: SearchEngines) => {
  const response = await api.post("searchengines", searchEnginesModelData);

  return response.data;
};

export const updateSearchEngines = async (searchEnginesModelData: SearchEngines) => {
  const response = await api.put("searchengines", searchEnginesModelData);
  return response.data;
};

export const deleteSearchEngines = async (searchEnginesModelDataTypeId: number) => {
  const response = await api.delete(`searchengines/${searchEnginesModelDataTypeId}`);
  return response.data;
};
