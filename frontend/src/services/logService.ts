import api, { ApiResponse } from "@/lib/api";

export interface ApiLog {
  id: number;
  direction: string;
  endpoint: string;
  method: string;
  requestBody?: string;
  responseBody?: string;
  statusCode: number;
  durationMs: number;
  createdAt: string;
}

interface LogPagedResult {
  items: ApiLog[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export const logService = {
  list: async (params?: { page?: number; pageSize?: number }) => {
    const { data } = await api.get<ApiResponse<LogPagedResult>>(
      "/logs",
      { params }
    );
    return data.data!;
  },
};
