"use server";
import { api } from "@/app/api/api";
import { LLMProvider } from "../_types/types";

export const createLlmProvider = async (llmData: LLMProvider) => {
  console.log("createLlmProvider,", llmData);
  const response = await api.post("llmproviders", llmData);
  return response.data;
};

export const updateLlmProvider = async (llmData: LLMProvider) => {
  const response = await api.put("llmproviders", llmData);
  return response.data;
};

export const deleteLlmProvider = async (llmId: number) => {
  const response = await api.delete(`llmproviders/${llmId}`);
  return response.data;
};
