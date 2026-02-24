import api, { ApiResponse } from "@/lib/api";
import {
  CreateUserRequest,
  UpdateUserRequest,
  UserListItem,
} from "@/lib/auth-types";

export const userService = {
  async list(): Promise<ApiResponse<UserListItem[]>> {
    const res = await api.get<ApiResponse<UserListItem[]>>("/users");
    return res.data;
  },

  async create(data: CreateUserRequest): Promise<ApiResponse<UserListItem>> {
    const res = await api.post<ApiResponse<UserListItem>>("/users", data);
    return res.data;
  },

  async update(
    id: string,
    data: UpdateUserRequest
  ): Promise<ApiResponse<UserListItem>> {
    const res = await api.put<ApiResponse<UserListItem>>(
      `/users/${id}`,
      data
    );
    return res.data;
  },

  async remove(id: string): Promise<ApiResponse<unknown>> {
    const res = await api.delete<ApiResponse<unknown>>(`/users/${id}`);
    return res.data;
  },
};
