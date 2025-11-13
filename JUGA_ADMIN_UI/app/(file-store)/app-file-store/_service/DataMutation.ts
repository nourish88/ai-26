"use server";
import { api } from "@/app/api/api";
import { FileStore } from "../_types/types";

export const createFireStore = async (emdeddingModelData: FileStore) => {
  const response = await api.post("applicationfilestores", emdeddingModelData);

  return response.data;
};

export const updateFileStore = async (emdeddingModelData: FileStore) => {
  const response = await api.put("applicationfilestores", emdeddingModelData);
  return response.data;
};

export const deleteFileStore = async (embeddingId: number) => {
  const response = await api.delete(`applicationfilestores/${embeddingId}`);
  return response.data;
};
