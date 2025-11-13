import { auth } from "@/auth";
import kopru from "kopru";
import { RequestConfig } from "kopru/dist/interfaces";

const api = kopru.create({
  baseURL: "http://localhost:5006/",
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  async (config: RequestConfig): Promise<RequestConfig> => {
    // Get the session on the server side
    const session = await auth();

    if (session?.accessToken) {
      // If the headers object doesn't exist, create it
      if (!config.headers) {
        config.headers = {};
      }
      // Add the Authorization header to the request
      config.headers["Authorization"] = `Bearer ${session.accessToken}`;
    }

    // Return the modified config to proceed with the request
    return config;
  },
  (error) => {
    // Handle any errors during the request setup
    return Promise.reject(error);
  }
);

export { api };
