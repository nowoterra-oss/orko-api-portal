import api, { ApiResponse } from "@/lib/api";

export interface AppSettingDto {
  id: number;
  key: string;
  value: string | null;
  description: string | null;
  category: string | null;
  updatedAt: string;
  updatedBy: string | null;
}

export interface SettingItemDto {
  key: string;
  value: string | null;
  description?: string | null;
  category?: string | null;
}

export interface SmtpTestResultDto {
  success: boolean;
  message: string | null;
}

export const settingsService = {
  async getAll(): Promise<ApiResponse<AppSettingDto[]>> {
    const res = await api.get<ApiResponse<AppSettingDto[]>>("/settings");
    return res.data;
  },

  async getByCategory(category: string): Promise<ApiResponse<AppSettingDto[]>> {
    const res = await api.get<ApiResponse<AppSettingDto[]>>(`/settings/${category}`);
    return res.data;
  },

  async update(settings: SettingItemDto[]): Promise<ApiResponse<unknown>> {
    const res = await api.put<ApiResponse<unknown>>("/settings", { settings });
    return res.data;
  },

  async testSmtp(): Promise<ApiResponse<SmtpTestResultDto>> {
    const res = await api.post<ApiResponse<SmtpTestResultDto>>("/settings/smtp-test");
    return res.data;
  },
};
