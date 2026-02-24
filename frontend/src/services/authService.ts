import api, { ApiResponse } from "@/lib/api";
import {
  LoginRequest,
  LoginResponse,
  SetupRequest,
  SetupStatus,
  UserInfo,
} from "@/lib/auth-types";

export const authService = {
  async login(data: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const res = await api.post<ApiResponse<LoginResponse>>(
      "/auth/login",
      data
    );
    return res.data;
  },

  async setup(data: SetupRequest): Promise<ApiResponse<LoginResponse>> {
    const res = await api.post<ApiResponse<LoginResponse>>(
      "/auth/setup",
      data
    );
    return res.data;
  },

  async getSetupStatus(): Promise<ApiResponse<SetupStatus>> {
    const res = await api.get<ApiResponse<SetupStatus>>(
      "/auth/setup-status"
    );
    return res.data;
  },

  async getMe(): Promise<ApiResponse<UserInfo>> {
    const res = await api.get<ApiResponse<UserInfo>>("/auth/me");
    return res.data;
  },
};
