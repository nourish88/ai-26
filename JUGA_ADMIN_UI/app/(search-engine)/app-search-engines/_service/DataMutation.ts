import { api } from "@/app/api/api";
import { AppSearchEngines } from "../_types/types";

export const createAppSearchEngines = async (appSearchEnginesModelData: AppSearchEngines) => {
  const response = await api.post("applicationsearchengines", appSearchEnginesModelData);

  return response.data;
};

export const updateAppSearchEngines = async (appSearchEnginesModelData: AppSearchEngines) => {
  const response = await api.put("applicationsearchengines", appSearchEnginesModelData);
  return response.data;
};

export const deleteAppSearchEngines = async (appSearchEnginesModelDataId: number) => {
  const response = await api.delete(`applicationsearchengines/${appSearchEnginesModelDataId}`);
  return response.data;
};
