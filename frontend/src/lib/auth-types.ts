export interface LoginRequest {
  email: string;
  password: string;
}

export interface SetupRequest {
  email: string;
  fullName: string;
  password: string;
}

export interface UserInfo {
  id: string;
  email: string;
  fullName: string;
  role: string;
}

export interface LoginResponse {
  token: string;
  user: UserInfo;
}

export interface SetupStatus {
  isSetupComplete: boolean;
  userCount: number;
}

export interface UserListItem {
  id: string;
  email: string;
  fullName: string;
  role: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateUserRequest {
  email: string;
  fullName: string;
  password: string;
  role: string;
}

export interface UpdateUserRequest {
  fullName?: string;
  role?: string;
  isActive?: boolean;
  password?: string;
}
