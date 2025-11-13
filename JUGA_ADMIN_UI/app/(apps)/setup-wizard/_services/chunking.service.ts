"use server";
import { api } from "@/app/api/api";
import { AppChunkingStrategies } from "../../apps/_types/types";
import { ApplicationChunkingStrategyDto } from "../_types/api-types";
type ServiceResponse<T> = {
  success: boolean;
  data?: T;
  message?: string;
};
type ChunkingPayload = Omit<ApplicationChunkingStrategyDto, "id">;

export const createAppChunkingStrategy = async (chunkingData: ChunkingPayload): Promise<ServiceResponse<ApplicationChunkingStrategyDto>> => {
  try {
    const response = await api.post("applicationchunkingstrategies", chunkingData);
    return { success: true, data: response.data };
  } catch (error: any) {
    console.error("Failed to create app chunking strategy:", error);
    return { success: false, message: "Failed to create the chunking strategy." };
  }
};
export const updateAppChunkingStrategy = async (chunkingData: AppChunkingStrategies) => {
  const response = await api.put("applicationchunkingstrategies", chunkingData);
  return response.data;
};

export const deleteAppChunkingStrategy = async (chunkingId: number) => {
  const response = await api.delete(`applicationchunkingstrategies/${chunkingId}`);
  return response.data;
};

export const fetchAllChunkingStrategies = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("applicationchunkingstrategies", {
    params: { pageIndex, pageSize },
  });
  return response.data;
};

export const fetchAllStrategies = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("chunkingstrategies", {
    params: { pageIndex, pageSize },
  });
  return response.data;
};
