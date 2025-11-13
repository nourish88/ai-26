"use server"
import { api } from "@/app/api/api";
import { LLMModel } from "../_types/types";

export const createLlmModel = async (llmModelData: LLMModel) => {
  const response = await api.post("llms", llmModelData);
  return response.data;
};

export const updateLlmModel = async (llmModelData: LLMModel) => {
  const response = await api.put("llms", llmModelData);
  return response.data;
};

export const deleteLlmModel = async (llmId: number) => {
  const response = await api.delete(`llms/${llmId}`);
  return response.data;
};
