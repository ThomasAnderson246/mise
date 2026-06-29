import { createContext, useContext, useState } from "react";
import type { ReactNode } from "react";

interface AuthUser {
    token: string
    email: string
    firstName: string
    lastName: string
    tenantId: string
}

interface AuthContextType {
    user: AuthUser | null
    setUser: (user: AuthUser | null) => void
    isAuthenticated: boolean
    logout: () => void
}

const AuthContext = createContext<AuthContextType | null>(null)

export function AuthProvider({children}: {children: ReactNode}) {
    const [user, setUser ] = useState<AuthUser | null>(null)

    function logout() {
        setUser(null)
    }

    return(
        <AuthContext.Provider value={{
            user, setUser, isAuthenticated: user !== null, logout
        }}>
            {children}
        </AuthContext.Provider>
    )
}

export function useAuth() {
    const context = useContext(AuthContext)
    if (!context)
        throw new Error('useAuth must be used within AuthProvider')
    return context
}