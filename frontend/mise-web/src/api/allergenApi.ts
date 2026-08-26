import axios from "axios";
import { BASE_URL, authHeaders } from "./config";

export interface AllergenTagItem{
    allergenId: string
    name: string
    description: string
    isMajor: boolean
    isSystemDefined: boolean
}

export interface CreateAllergenTagRequest{
    name: string
    description: string
    isMajor: boolean
}

export interface UpdateAllergenTagRequest {
    name?: string
    description?: string
    isSystemDefined?: boolean
}

export async function getAllAllergens(token: string) : Promise<AllergenTagItem[]>{
    const response = await axios.get(`${BASE_URL}/api/allergentag`, authHeaders(token))
    return response.data.data
}

export async function getAllergen(token: string, allergenId: string): Promise<AllergenTagItem>{
    const response = await axios.get(`${BASE_URL}/api/allergentag/${allergenId}`, authHeaders(token))
    return response.data.data
}

export async function createAllergenTag(token: string, request: CreateAllergenTagRequest): Promise<AllergenTagItem>{
    const response = await axios.post(`${BASE_URL}/api/allergentag`, request, authHeaders(token))
    return response.data.data
}

export async function updateAllergenTag(token: string, allergenId: string, request: UpdateAllergenTagRequest ): Promise<AllergenTagItem>{
    const response = await axios.post(`${BASE_URL}/api/allergentag/${allergenId}`, request, authHeaders(token))
    return response.data.data
}

export async function deleteAllergenTag(token: string, allergenId: string): Promise<void>{
    await axios.delete(`${BASE_URL}/api/allergentag/${allergenId}`, authHeaders(token))
}