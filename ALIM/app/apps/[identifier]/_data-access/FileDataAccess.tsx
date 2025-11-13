// _data-access/fileDataAccess.ts

// Define the TypeScript types that match your API's response structure
export interface FileType {
  id: number;
  identifier: string;
}

interface FileTypesApiResponse {
  result: {
    items: FileType[];
    size: number;
    index: number;
    count: number;
    pages: number;
    hasNext: boolean;
    hasPrevious: boolean;
  };
}

// This is a server-side function to fetch the list of available file types
export const fetchFileTypes = async (): Promise<FileType[]> => {
  try {
    // Use an environment variable for your base API URL as a best practice
    const res = await fetch(`${process.env.AI_ADMIN_URL}/filetypes?pageIndex=0&pageSize=10`, {
      headers: { accept: "application/json" },
      // Cache this result for an hour, as file types likely don't change often
      next: { revalidate: 3600 },
    });

    if (!res.ok) {
      console.error("Failed to fetch file types from API");
      return [];
    }

    const data: FileTypesApiResponse = await res.json();
    return data.result.items;
  } catch (error) {
    console.error("Error processing file types:", error);
    return [];
  }
};
