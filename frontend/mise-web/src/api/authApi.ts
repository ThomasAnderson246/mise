import axios from 'axios'

const BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:7144"

export interface LoginRequest {
    email: string
    password: string
    tenantId: string
}

export interface LoginResponse {
    token: string
    email: string
    firstName: string
    lastName: string
    tenantId: string
    role: string
    permissions: string[]
}

export async function Login(request: LoginRequest): Promise<LoginResponse>{
    const response = await axios.post(`${BASE_URL}/api/auth/login`, request, {
        withCredentials: true
    })
    return response.data.data
}

export async function getPermissions(token: string): Promise<string[]> {
    const response = await axios.get(`${BASE_URL}/api/auth/permissions`, 
        {withCredentials: true,
        headers:{
            Authorization: `Bearer ${token}`
        }
        })
    return response.data.data
}