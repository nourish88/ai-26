import { api } from "@/app/api/api";
import { FileStore } from "../_types/types";

export const createFireStore = async (emdeddingModelData: FileStore) => {
  const response = await api.post("fileStores", emdeddingModelData);

  return response.data;
};

export const updateFileStore = async (emdeddingModelData: FileStore) => {
  const response = await api.put("fileStores", emdeddingModelData);
  return response.data;
};

export const deleteFileStore = async (embeddingId: number) => {
  const response = await api.delete(`fileStores/${embeddingId}`);
  return response.data;
};
