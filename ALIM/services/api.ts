import kopru from "kopru";

export const api = kopru.create({
  baseURL: "http://127.0.0.1:8000/",
});
