import axiosClient from "./axios";

export const api = {
  get: <T>(url: string, params?: object): Promise<T> =>
    axiosClient.get(url, { params }),

  post: <T>(url: string, data: object): Promise<T> =>
    axiosClient.post(url, data),

  put: <T>(url: string, data: object): Promise<T> => axiosClient.put(url, data),

  patch: <T>(url: string, data: object): Promise<T> =>
    axiosClient.patch(url, data),

  delete: <T>(url: string): Promise<T> => axiosClient.delete(url),
};
