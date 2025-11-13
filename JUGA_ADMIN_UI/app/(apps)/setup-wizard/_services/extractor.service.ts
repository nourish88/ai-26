"use server";
import { DataManagerExtractionEngineType } from "@/app/(extraction)/data-manager/_types/types";
import { api } from "@/app/api/api";

export const createExtractorEngine = async (extractorData: Omit<DataManagerExtractionEngineType, "id">) => {
  const response = await api.post("applicationextractorengine", extractorData);
  return response.data;
};

export const updateExtractorEngine = async (extractorData: DataManagerExtractionEngineType) => {
  const response = await api.put("applicationextractorengine", extractorData);
  return response.data;
};

export const deleteExtractorEngine = async (extractorId: number) => {
  const response = await api.delete(`applicationextractorengine/${extractorId}`);
  return response.data;
};

export const extractorEngineTypes = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("extractorenginetypes", {
    params: { pageIndex, pageSize },
  });
  return response.data;
};
