import { api } from "@/app/api/api";
import { DataManagerExtractionEngineType } from "../_types/types";

export const createDataManagerExtractorEngineType = async (engineTypeModelData: DataManagerExtractionEngineType) => {
  const response = await api.post("applicationextractorengine", engineTypeModelData);

  return response.data;
};

export const updateDataManagerExtractorEngineType = async (engineTypeModelData: DataManagerExtractionEngineType) => {
  const response = await api.put("applicationextractorengine", engineTypeModelData);
  return response.data;
};

export const deleteDataManagerExtractorEngineType = async (dataManagerExtractionEngineTypeId: number) => {
  const response = await api.delete(`applicationextractorengine/${dataManagerExtractionEngineTypeId}`);
  return response.data;
};
