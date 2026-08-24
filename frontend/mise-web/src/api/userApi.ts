import axios from 'axios'
import { BASE_URL, authHeaders } from './config'

export interface UserItem {
    userId: string
    email: string
    firstName: string
    lastName: string
    role: string
    status: string
    roles: string[]
    mustChangePassword: boolean
    lastLoginAt: string | null
    createdAt: string
}

export interface InviteUserRequest {
    email: string
    firstName: string
    lastName: string
    unitPreference: string
    roleIds: string[]
}

export interface InviteUserResponse {
    user: UserItem
    temporaryPassword: string
}

export async function getUsers(token: string): Promise<UserItem[]>{
    const response = await axios.get(`${BASE_URL}/api/user`, authHeaders(token))
    return response.data.data
}

export async function inviteUser(token: string, request: InviteUserRequest): Promise<InviteUserResponse>{
    const response = await axios.post(`${BASE_URL}/api/user/invite`, request, authHeaders(token))
    return response.data.data
}

export async function deactivateUser(token: string, userId:string): Promise<void>{
    await axios.post(`${BASE_URL}/api/user/${userId}/deactivate`, {}, authHeaders(token))
}

export async function reactivateUser(token: string, userId: string): Promise<void> {
    await axios.post(`${BASE_URL}/api/user/${userId}/reactivate`, {}, authHeaders(token))
}

export async function assignRole(token: string, userId: string, roleId:string): Promise<void>{
    await axios.post(`${BASE_URL}/api/user/{${userId}/roles`, {roleId}, authHeaders(token))
}

export async function removeRole(token: string, userId: string, roleId: string): Promise<void>{
    await axios.delete(`${BASE_URL}/api/user/${userId}/roles/${roleId}`, authHeaders(token))
}