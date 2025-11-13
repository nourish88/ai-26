"use server";
import { api } from "@/app/api/api";
import { AppChunkingStrategies } from "../_types/types";

export const createAppChunkingStrategy = async (data: Omit<AppChunkingStrategies, "id">) => {
  const res = await api.post(
    "applications", // 1st argument: URL
    data
  );
  return res.data;
};

export const updateAppChunkingStrategy = async (data: AppChunkingStrategies) => {
  const res = await api.put(
    "applications", // 1st argument: URL
    data // 2nd argument: Request body/data
  );
  return res.data;
};

export const deleteAppChunkingStrategy = async (id: number) => {
  const res = await api.delete(`applications/${id}`, {
    // 3rd argument: Configuration object
  });
  return res.data;
};
