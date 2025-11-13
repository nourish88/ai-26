"use server";
import { api } from "@/app/api/api";

export const fetchFileStores = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("applicationfilestores", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
