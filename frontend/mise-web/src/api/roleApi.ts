import axios from "axios";
import { BASE_URL, authHeaders } from "./config";

export interface RolePermission {
    permissionId: string
    name: string
    resource: string
    action: string
}

export interface Role {
    roleId: string
    name: string
    isSystemRole: boolean
    permissions: RolePermission[]
}

export interface Permission {
    permissionId: string
    name: string
    resource: string
    action: string
    description: string | null
}

export async function getRoles(token: string): Promise<Role[]>{
    const response = await axios.get(`${BASE_URL}/api/role`, authHeaders(token))
    return response.data.data
}

export async function getAllPermissions(token: string): Promise<Permission[]> {
    const response = await axios.get(`${BASE_URL}/api/permission`, authHeaders(token))
    console.log('Permissions response:', response.data)
    return response.data.data
}

export async function addPermissionToRole(token: string, roleId: string, permissionId: string): Promise<void>{
    console.log('Adding permission:', permissionId, 'to role:', roleId)
    await axios.post(`${BASE_URL}/api/role/${roleId}/permissions`, {permissionId}, authHeaders(token))
}

export async function removePermissionFromRole(token: string, roleId: string, permissionId: string): Promise<void>{
    await axios.delete(`${BASE_URL}/api/role/${roleId}/permissions/${permissionId}`, authHeaders(token))
}

export async function createRole(token: string, name: string): Promise<Role> {
    const response = await axios.post (`${BASE_URL}/api/role`, {name}, authHeaders(token))
    return response.data.data
}