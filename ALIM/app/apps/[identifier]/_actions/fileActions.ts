"use server";

import { auth } from "@/auth";

// A structured return type for our action, essential for useActionState
export interface UploadActionState {
  data?: any;
  success: boolean;
  message: string;
}

export async function uploadFilesAction(identifier: string, formData: FormData): Promise<UploadActionState> {
  const endpoint = "http://localhost:5006/files/user/storage";
  const session = await auth();

  // Check if there are any files to upload before making the request
  if (!formData.has("files[0].File")) {
    return { success: false, message: "Please select at least one file to upload." };
  }

  try {
    const response = await fetch(endpoint, {
      method: "POST",
      body: formData,
      // CRITICAL FIX: Do NOT manually set the 'Content-Type' header for FormData.
      // The `fetch` API correctly sets it along with the required 'boundary' string.
      headers: {
        Authorization: `Bearer ${session?.accessToken}`,
        "app-identifier": identifier,
      },
    });

    if (!response.ok) {
      // Try to parse error details from the backend if available
      const errorData = await response.json().catch(() => ({ message: "An unknown error occurred on the storage server." }));
      console.error("Upload failed with status:", response.status, errorData);
      return { success: false, message: `Upload failed: ${errorData.message || response.statusText}` };
    }

    // Success
    const responseData = await response.json();
    console.log("Upload successful:", responseData);
    return { data: responseData, success: true, message: "Dosyalar başarıyla yüklendi!" };
  } catch (error) {
    console.error("Dosya yükleme esnasında hata oluştu:", error);
    // This specific error check helps debug if the backend server is down
    if (error instanceof TypeError && error.message.includes("fetch failed")) {
      return { success: false, message: "Connection Refused: Storage sunucusu kapalı olabilir." };
    }
    return { success: false, message: "An unexpected server-side error occurred." };
  }
}

export async function deleteFile(fileId: string, identifier: string) {
  console.log("fileId", fileId, "identifier", identifier);
  const session = await auth();
  try {
    const response = await fetch(`${process.env.AI_ORCH_URL}/files/user?file_id=${fileId}`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        "app-identifier": identifier,
        Authorization: `Bearer ${session?.accessToken}`,
      },
    });
    if (!response.ok) {
      throw new Error(`Failed to delete file: ${response.statusText}`);
    }
    return { success: true, message: "Dosya başarıyla silindi!" };
  } catch (error) {
    console.error("Dosya silme esnasında hata oluştu:", error);
    // This specific error check helps debug if the backend server is down
    if (error instanceof TypeError && error.message.includes("fetch failed")) {
      return { success: false, message: "Connection Refused: Storage sunucusu kapalı olabilir." };
    }
    return { success: false, message: "An unexpected server-side error occurred." };
  }
}
