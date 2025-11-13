import { api } from "@/app/api/api";
import { ExtractionEngineType } from "../_types/types";

export const createExtractorEngineType = async (engineTypeModelData: ExtractionEngineType) => {
  const response = await api.post("extractorenginetypes", engineTypeModelData);

  return response.data;
};

export const updateExtractorEngineType = async (engineTypeModelData: ExtractionEngineType) => {
  const response = await api.put("extractorenginetypes", engineTypeModelData);
  return response.data;
};

export const deleteExtractorEngineType = async (extractionEngineTypeId: number) => {
  const response = await api.delete(`extractorenginetypes/${extractionEngineTypeId}`);
  return response.data;
};
