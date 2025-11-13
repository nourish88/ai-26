"use server";

import { api } from "@/app/api/api";

export const createApplicationMcp = async (payload: any) => {
  console.log("🟢 SERVER ACTION CALLED: createapplicationllms");
  console.log("📦 Received data:", JSON.stringify(payload, null, 2));

  try {
    console.log("🌐 Making POST request to applicationllms endpoint");
    const response = await api.post("applicationmcpservers", payload);
    console.log("✅ API Response:", response.data);
    return response.data;
  } catch (error: any) {
    console.error("❌ Error in createapplicationllms:", error);
    console.error("❌ Error response:", error.response?.data);
    console.error("❌ Error message:", error.message);
    throw error; // Re-throw to propagate to client
  }
};
