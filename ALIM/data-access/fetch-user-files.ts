import { auth } from "@/auth";

export const fetchAllUserFiles = async ({ params }: { params: Promise<{ identifier: string }> }) => {
  const { identifier } = await params;
  console.log("identifer", identifier);
};

// The fetch itself handles caching via next.revalidate option
export const fetchAllFiles = async (identifier: string) => {
  console.log("identifer", identifier);
  const session = await auth();
  console.log("session?.accessToken", session?.accessToken);
  if (!session?.accessToken) {
    return [];
  }

  const response = await fetch(`${process.env.AI_ORCH_URL}/files/user`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${session?.accessToken}`,
      "app-identifier": identifier,
    },
    next: {
      tags: ["user-files"],
    },
  });
  console.log("response", response);
  // if (!response.ok) {
  //   throw new Error(`Failed to fetch applications: ${response.statusText}`);
  // }

  return response.json();
};
