"use server";
import { api } from "@/app/api/api";
import type { Application } from "../_types/types";

export const createApplication = async (applicationData: Omit<Application, "id">) => {
  try {
    console.log("first", applicationData);
    const response = await api.post("applications", applicationData);
    // This will return the full Application object from the backend
    return { success: true, data: response.data as Application };
  } catch (error) {
    console.error("Failed to create application:", error);
    // You might want to pass back more specific error info from the API if available
    return { success: false, message: "Failed to create the application. Check server logs." };
  }
};

export const updateApplication = async (applicationData: Application) => {
  const response = await api.put("applications", applicationData);
  return response.data;
};

export const deleteApplication = async (applicationId: number) => {
  const response = await api.delete(`applications/${applicationId}`);
  return response.data;
};

export const getApplications = async (pageIndex = 0, pageSize = 10) => {
  const response = await api.get("applications", {
    params: { pageIndex, pageSize },
  });
  return response.data.result;
};
