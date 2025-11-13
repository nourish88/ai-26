"use server";
import { api } from "@/app/api/api";

export const fetchAppChunkingStrageties = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("applications", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
