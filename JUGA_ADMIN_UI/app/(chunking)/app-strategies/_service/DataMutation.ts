import { api } from "@/app/api/api";
import { AppChunkingStrategies } from "../_types/types";

export const createAppChunkingStrategy = async (appChunkingStrategyModelData: AppChunkingStrategies) => {
  const response = await api.post("applicationchunkingstrategies", appChunkingStrategyModelData);

  return response.data;
};

export const updateAppChunkingStrategy = async (appChunkingStrategyModelData: AppChunkingStrategies) => {
  const response = await api.put("applicationchunkingstrategies", appChunkingStrategyModelData);
  return response.data;
};

export const deleteAppChunkingStrategy = async (appChunkingStrategyTypeId: number) => {
  const response = await api.delete(`applicationchunkingstrategies/${appChunkingStrategyTypeId}`);
  return response.data;
};
