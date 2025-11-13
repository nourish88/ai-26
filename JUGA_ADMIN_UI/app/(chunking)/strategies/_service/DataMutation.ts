import { api } from "@/app/api/api";
import { ChunkingStrategies } from "../_types/types";

export const createChunkingStrategy = async (chunkingStrategyModelData: ChunkingStrategies) => {
  const response = await api.post("chunkingstrategies", chunkingStrategyModelData);

  return response.data;
};

export const updateChunkingStrategy = async (chunkingStrategyModelData: ChunkingStrategies) => {
  const response = await api.put("chunkingstrategies", chunkingStrategyModelData);
  return response.data;
};

export const deleteChunkingStrategy = async (chunkingStrategyTypeId: number) => {
  const response = await api.delete(`chunkingstrategies/${chunkingStrategyTypeId}`);
  return response.data;
};
