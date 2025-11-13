import { api } from "@/app/api/api";

// Fetches all available LLMs
export const fetchAllMcpServers = async () => {
  try {
    const response = await api.get("mcpservers?pageIndex=0&pageSize=1000");
    return response.data;
  } catch (error: any) {
    console.error("Error fetching existing models:", error.response?.data || error.message);
    throw new Error("Mevcut modeller getirilirken bir hata oluştu.");
  }
};
