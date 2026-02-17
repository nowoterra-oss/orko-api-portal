import api, { ApiResponse } from "@/lib/api";
import { AllowedTransitions, StatusHistory } from "@/lib/types";

export const statusService = {
  getAllowedTransitions: async (declarationId: string) => {
    const { data } = await api.get<ApiResponse<AllowedTransitions>>(
      `/declarations/${declarationId}/status/transitions`
    );
    return data.data!;
  },

  update: async (declarationId: string, newStatus: string, note?: string) => {
    const { data } = await api.put<ApiResponse<StatusHistory>>(
      `/declarations/${declarationId}/status`,
      { newStatus, note }
    );
    return data;
  },
};
