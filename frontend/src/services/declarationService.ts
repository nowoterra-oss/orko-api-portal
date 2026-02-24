import api, { ApiResponse } from "@/lib/api";
import { DeclarationDetail } from "@/lib/types";

export const declarationService = {
  get: async (id: string) => {
    const { data } = await api.get<ApiResponse<DeclarationDetail>>(
      `/declarations/${id}`
    );
    return data.data!;
  },

  update: async (id: string, declarationData: string) => {
    const { data } = await api.put<ApiResponse<object>>(
      `/declarations/${id}`,
      { declarationData }
    );
    return data;
  },

  sendToEvrim: async (id: string) => {
    const { data } = await api.post<ApiResponse<{ evrimDeclarationId: string }>>(
      `/declarations/${id}/send`
    );
    return data;
  },

  parseFile: async (id: string, fileContent: string, fileFormat: "json" | "xml") => {
    const { data } = await api.post<ApiResponse<Record<string, unknown>>>(
      `/declarations/${id}/parse-file`,
      { fileContent, fileFormat }
    );
    return data;
  },

  uploadAndSend: async (id: string, fileContent: string, fileFormat: "json" | "xml") => {
    const { data } = await api.post<ApiResponse<{ evrimDeclarationId: string }>>(
      `/declarations/${id}/upload-and-send`,
      { fileContent, fileFormat }
    );
    return data;
  },
};
