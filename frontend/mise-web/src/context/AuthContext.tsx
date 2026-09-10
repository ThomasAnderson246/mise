import { createContext, useContext, useState } from "react";
import type { ReactNode } from "react";

interface AuthUser {
  token: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  tenantId: string;
  role: string;
  permissions: string[];
}

interface AuthContextType {
  user: AuthUser | null;
  setUser: (user: AuthUser | null) => void;
  isAuthenticated: boolean;
  logout: () => void;
  hasPermission: (resource: string, action: string) => boolean;
}

const STORAGE_KEY = "mise_user";

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUserState] = useState<AuthUser | null>(() => {
    try {
      const stored = localStorage.getItem(STORAGE_KEY);
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  });

  function setUser(user: AuthUser | null) {
    if (user) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    } else {
      localStorage.removeItem(STORAGE_KEY);
    }
    setUserState(user);
  }

  function logout() {
    localStorage.removeItem(STORAGE_KEY);
    setUserState(null);
  }

  function hasPermission(resource: string, action: string): boolean {
    return user?.permissions.includes(`${resource}.${action}`) ?? false;
  }

  return (
    <AuthContext.Provider
      value={{
        user,
        setUser,
        isAuthenticated: user !== null,
        logout,
        hasPermission,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
}
