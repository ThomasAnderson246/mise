import axios from 'axios'
import { BASE_URL, authHeaders } from './config'


export interface LoginRequest {
    email: string
    password: string
    tenantId: string
}

export interface LoginResponse {
    token: string
    userId: string
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
    const response = await axios.get(`${BASE_URL}/api/auth/permissions`, authHeaders(token))
    return response.data.data
}