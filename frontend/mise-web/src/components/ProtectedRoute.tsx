import { Navigate } from "react-router-dom";
import type { ReactNode } from "react";
import { useAuth } from "../context/AuthContext";

interface ProtectedRouteProps{
    children: ReactNode
    slug: string
}

function ProtectedRoute({children, slug} : ProtectedRouteProps) {
    const { isAuthenticated} = useAuth()

    if (!isAuthenticated)
        return <Navigate to={`/${slug}/login`} replace/>

    return <>{children}</>
}

export default ProtectedRoute