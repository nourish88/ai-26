"use server"
import { api } from "@/app/api/api";
import { EmbeddingModel } from "../_types/types";

export const createLlmModel = async (emdeddingModelData: EmbeddingModel) => {
  const response = await api.post("embeddings", emdeddingModelData);

  return response.data;
};

export const updateLlmModel = async (emdeddingModelData: EmbeddingModel) => {
  const response = await api.put("embeddings", emdeddingModelData);
  return response.data;
};

export const deleteLlmModel = async (embeddingId: number) => {
  const response = await api.delete(`embeddings/${embeddingId}`);
  return response.data;
};
