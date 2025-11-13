"use server";
import { api } from "@/app/api/api";
import type { EmbeddingModel } from "../_types/types";
import { ApplicationEmbeddingDto, EmbeddingDto, PageResponse } from "../_types/api-types";

export const createEmbedding = async (embeddingData: Omit<EmbeddingModel, "id">) => {
  const response = await api.post("embeddings", embeddingData);
  return response.data;
};

export const updateEmbedding = async (embeddingData: EmbeddingModel) => {
  const response = await api.put("embeddings", embeddingData);
  return response.data;
};

export const deleteEmbedding = async (embeddingId: number) => {
  const response = await api.delete(`embeddings/${embeddingId}`);
  return response.data;
};

export const getEmbeddingModels = async () => {
  // This uses the endpoint you provided
  const response = await api.get("embeddings", {
    params: { pageIndex: 0, pageSize: 10 }, // Fetch a large number for the dropdown
  });

  return response.data;
};

type ServiceResponse<T> = {
  success: boolean;
  data?: T;
  message?: string;
};

/**
 * Creates a link between an application and an embedding model.
 */
export const createApplicationEmbedding = async (payload: { applicationId: number; embeddingId: number }): Promise<ServiceResponse<ApplicationEmbeddingDto>> => {
  try {
    // This hits the endpoint from your curl command
    const response = await api.post("applicationembeddings", payload);
    return { success: true, data: response.data };
  } catch (error: any) {
    return { success: false, message: "Failed to link embedding model." };
  }
};
