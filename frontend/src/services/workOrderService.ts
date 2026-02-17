import api, { ApiResponse, PagedResult } from "@/lib/api";
import { WorkOrder, WorkOrderDetail } from "@/lib/types";

export const workOrderService = {
  list: async (params?: {
    page?: number;
    pageSize?: number;
    status?: string;
    type?: string;
    search?: string;
  }) => {
    const { data } = await api.get<ApiResponse<PagedResult<WorkOrder>>>(
      "/work-orders",
      { params }
    );
    return data.data!;
  },

  getByFileNumber: async (fileNumber: string) => {
    const { data } = await api.get<ApiResponse<WorkOrderDetail>>(
      `/work-orders/${fileNumber}`
    );
    return data.data!;
  },
};
