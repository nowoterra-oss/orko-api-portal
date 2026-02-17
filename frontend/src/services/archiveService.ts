import api, { ApiResponse } from "@/lib/api";
import { Archive } from "@/lib/types";

export const archiveService = {
  list: async (declarationId: string) => {
    const { data } = await api.get<ApiResponse<Archive[]>>(
      `/declarations/${declarationId}/archives`
    );
    return data.data!;
  },

  upload: async (
    declarationId: string,
    file: { fileName: string; base64Data: string; contentType?: string; documentType?: string }
  ) => {
    const { data } = await api.post<ApiResponse<Archive>>(
      `/declarations/${declarationId}/archives`,
      file
    );
    return data;
  },

  sendToEvrim: async (declarationId: string, archiveId: string) => {
    const { data } = await api.post<ApiResponse<object>>(
      `/declarations/${declarationId}/archives/${archiveId}/send`
    );
    return data;
  },
};
