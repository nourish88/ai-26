// Create this file: _services/index.service.ts
"use server";
import { api } from "@/app/api/api";

// Assuming the API returns some data about the created index
export const createIndex = async (applicationId: number) => {
  const response = await api.post("indexmanagement/createindex", { applicationId });
  return response.data;
};
