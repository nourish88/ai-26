"use server";
import { api } from "@/app/api/api";
import type { LLMModel } from "../_types/types";

export const createapplicationllms = async (llmModelData: Omit<LLMModel, "id"> & { applicationId: number }) => {
  console.log("🟢 SERVER ACTION CALLED: createapplicationllms");
  console.log("📦 Received data:", JSON.stringify(llmModelData, null, 2));

  try {
    console.log("🌐 Making POST request to applicationllms endpoint");
    const response = await api.post("applicationllms", llmModelData);
    console.log("✅ API Response:", response.data);
    return response.data;
  } catch (error: any) {
    console.error("❌ Error in createapplicationllms:", error);
    console.error("❌ Error response:", error.response?.data);
    console.error("❌ Error message:", error.message);
    throw error; // Re-throw to propagate to client
  }
};

// Updates an existing LLM configuration
export const updateLlmModel = async (llmModelData: LLMModel) => {
  try {
    console.log(`Attempting to update LLM model ${llmModelData.id}`);
    const response = await api.put("llmmodels", llmModelData);
    return response.data;
  } catch (error: any) {
    console.error(`Error updating LLM model ${llmModelData.id}:`, error.response?.data || error.message);
    throw new Error("Model yapılandırması güncellenirken bir hata oluştu.");
  }
};

// Deletes an LLM configuration
export const deleteLlmModel = async (llmId: number) => {
  try {
    console.log(`Attempting to delete LLM model ${llmId}`);
    const response = await api.delete(`llmmodels/${llmId}`);
    return response.data;
  } catch (error: any) {
    console.error(`Error deleting LLM model ${llmId}:`, error.response?.data || error.message);
    throw new Error("Model yapılandırması silinirken bir hata oluştu.");
  }
};

// Fetches all available LLMs
export const fetchExistingModels = async () => {
  try {
    const response = await api.get("llms?pageIndex=0&pageSize=1000");
    return response.data;
  } catch (error: any) {
    console.error("Error fetching existing models:", error.response?.data || error.message);
    throw new Error("Mevcut modeller getirilirken bir hata oluştu.");
  }
};
