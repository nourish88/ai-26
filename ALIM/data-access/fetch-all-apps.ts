import { auth } from "@/auth";

// The fetch itself handles caching via next.revalidate option
export const fetchAllApplications = async () => {
  const session = await auth();
  console.log("session?.accessToken", session?.accessToken);
  if (!session?.accessToken) {
    return [];
  }

  const response = await fetch(`${process.env.AI_ORCH_URL}/applications`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      "app-identifier": "test-app",
      Authorization: `Bearer ${session?.accessToken}`,
    },
    next: {
      tags: ["applications-list"],
      revalidate: 3600, // Cache for 1 hour - this makes subsequent fetches instant
    },
  });

  // if (!response.ok) {
  //   throw new Error(`Failed to fetch applications: ${response.statusText}`);
  // }

  return response.json();
};
