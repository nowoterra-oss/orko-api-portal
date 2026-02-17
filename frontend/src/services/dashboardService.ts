import api, { ApiResponse } from "@/lib/api";
import { DashboardSummary } from "@/lib/types";

export const dashboardService = {
  getSummary: async () => {
    const { data } = await api.get<ApiResponse<DashboardSummary>>(
      "/dashboard/summary"
    );
    return data.data!;
  },
};
