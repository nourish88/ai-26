// in _services/file-store.service.ts

"use server";

import { api } from "@/app/api/api";
// Import the necessary types
import type { ApplicationFileStoreDto, FileStoreDto, PageResponse } from "../_types/api-types";

// Defines the shape for the API response
type ServiceResponse<T> = {
  success: boolean;
  data?: T;
  message?: string;
};

/**
 * Fetches the master list of all available file stores.
 */
export const getFileStores = async (pageIndex: number = 0, pageSize: number = 10): Promise<ServiceResponse<PageResponse<FileStoreDto>>> => {
  try {
    // Corrected to fetch from the master list endpoint
    const response = await api.get("filestores", {
      params: { pageIndex, pageSize },
    });
    // The API wraps the list in a 'result' object
    return { success: true, data: response.data.result };
  } catch (error: any) {
    console.error("Failed to get file stores:", error);
    return { success: false, message: "Failed to fetch file stores." };
  }
};

/**
 * Creates a link between an application and a file store.
 */
export const createApplicationFileStore = async (payload: { applicationId: number; fileStoreId: number }): Promise<ServiceResponse<ApplicationFileStoreDto>> => {
  try {
    // The endpoint for creating the link
    const response = await api.post("applicationfilestores", payload);
    return { success: true, data: response.data };
  } catch (error: any) {
    console.error("Failed to create application file store link:", error);
    return { success: false, message: "Failed to link file store." };
  }
};
