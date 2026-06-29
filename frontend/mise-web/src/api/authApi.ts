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
}

export async function Login(request: LoginRequest): Promise<LoginResponse>{
    const response = await axios.post(`${BASE_URL}/api/auth/login`, request, {
        withCredentials: true
    })
    return response.data.data
}