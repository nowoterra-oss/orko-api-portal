"use client";

import {
  createContext,
  useContext,
  useEffect,
  useState,
  useCallback,
  ReactNode,
} from "react";
import { useRouter } from "next/navigation";
import { UserInfo } from "@/lib/auth-types";
import { authService } from "@/services/authService";

interface AuthContextType {
  user: UserInfo | null;
  token: string | null;
  loading: boolean;
  login: (token: string, user: UserInfo) => void;
  logout: () => void;
  hasRole: (...roles: string[]) => boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

function setTokenCookie(token: string | null) {
  if (token) {
    document.cookie = `auth-token=${token}; path=/; max-age=${60 * 60 * 8}; SameSite=Lax`;
  } else {
    document.cookie = "auth-token=; path=/; max-age=0";
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const savedToken = localStorage.getItem("auth-token");
    const savedUser = localStorage.getItem("auth-user");

    if (savedToken && savedUser) {
      setToken(savedToken);
      setUser(JSON.parse(savedUser));
      setTokenCookie(savedToken);

      // Validate token by calling /me
      authService
        .getMe()
        .then((res) => {
          if (res.success && res.data) {
            setUser(res.data);
            localStorage.setItem("auth-user", JSON.stringify(res.data));
          } else {
            // Token invalid
            localStorage.removeItem("auth-token");
            localStorage.removeItem("auth-user");
            setTokenCookie(null);
            setToken(null);
            setUser(null);
          }
        })
        .catch(() => {
          localStorage.removeItem("auth-token");
          localStorage.removeItem("auth-user");
          setTokenCookie(null);
          setToken(null);
          setUser(null);
        })
        .finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
  }, []);

  const login = useCallback((newToken: string, newUser: UserInfo) => {
    setToken(newToken);
    setUser(newUser);
    localStorage.setItem("auth-token", newToken);
    localStorage.setItem("auth-user", JSON.stringify(newUser));
    setTokenCookie(newToken);
  }, []);

  const logout = useCallback(() => {
    setToken(null);
    setUser(null);
    localStorage.removeItem("auth-token");
    localStorage.removeItem("auth-user");
    setTokenCookie(null);
    router.push("/login");
  }, [router]);

  const hasRole = useCallback(
    (...roles: string[]) => {
      if (!user) return false;
      return roles.includes(user.role);
    },
    [user]
  );

  return (
    <AuthContext.Provider value={{ user, token, loading, login, logout, hasRole }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}
