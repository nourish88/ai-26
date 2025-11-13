// in a new file, e.g., _services/types.service.ts
"use server";

import { api } from "@/app/api/api";

// A generic type for these responses since their structure is identical
type TypeDto = {
  id: number;
  identifier: string;
};

type ServiceResponse<T> = {
  success: boolean;
  data?: T;
  message?: string;
};

// Fetches from /applicationtypes
export const getApplicationTypes = async (): Promise<ServiceResponse<{ items: TypeDto[] }>> => {
  try {
    const response = await api.get("applicationtypes", { params: { pageIndex: 0, pageSize: 100 } });
    console.log("response", response);
    return { success: true, data: response.data.result };
  } catch (error: any) {
    return { success: false, message: "Failed to fetch application types." };
  }
};

// Fetches from /memorytypes
export const getMemoryTypes = async (): Promise<ServiceResponse<{ items: TypeDto[] }>> => {
  try {
    const response = await api.get("memorytypes", { params: { pageIndex: 0, pageSize: 100 } });
    return { success: true, data: response.data.result };
  } catch (error: any) {
    return { success: false, message: "Failed to fetch memory types." };
  }
};

// Fetches from /outputtypes
export const getOutputTypes = async (): Promise<ServiceResponse<{ items: TypeDto[] }>> => {
  try {
    const response = await api.get("outputtypes", { params: { pageIndex: 0, pageSize: 100 } });
    return { success: true, data: response.data.result };
  } catch (error: any) {
    return { success: false, message: "Failed to fetch output types." };
  }
};
