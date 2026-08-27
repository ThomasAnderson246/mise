import axios from "axios";
import { BASE_URL, authHeaders } from "./config";

export interface IngredientItem {
    ingredientId: string
    name: string
    category: string | null
    defaultUnitTypeId: string | null
    defaultUnitTypeName: string | null
    isNonConvertible: boolean
    allergens: {
        allergenId: string
        name: string
        isMajor: boolean
    }[]
}


export interface CreateIngredientRequest {
    name: string
    category: string | null
    defaultUnitTypeId: string | null
    isNonConvertible: boolean
    allergenIds: string[]
}

export interface UpdateIngredientRequest {
    name?: string
    category?: string | null
    defaultUnitTypeId?: string | null
    isNonConvertible?: boolean
    allergenIds?: string[]
}

export async function getIngredients(token: string): Promise<IngredientItem[]>{
    const response = await axios.get(`${BASE_URL}/api/ingredient`, authHeaders(token))
    return response.data.data
}

export async function getIngredientById(token: string, ingredientId: string) :  Promise<IngredientItem>{
    const response = await axios.get(`${BASE_URL}/api/ingredient/${ingredientId}`, authHeaders(token))
    return response.data.data
}

export async function searchIngredients(token: string, term: string): Promise<IngredientItem[]>{
    const response = await axios.get(`${BASE_URL}/api/ingredient/search?term=${encodeURIComponent(term)}`, authHeaders(token))
    return response.data.data
}

export async function createIngredient(token: string, request: CreateIngredientRequest): Promise<IngredientItem>{
    const response = await axios.post(`${BASE_URL}/api/ingredient`, request, authHeaders(token))
    return response.data.data
}

export async function updateIngredient(token: string, ingredientId: string, request: UpdateIngredientRequest): Promise<IngredientItem>{
    const response = await axios.put(`${BASE_URL}/api/ingredient/${ingredientId}`, request, authHeaders(token))
    return response.data.data
}

export async function deleteIngredient(token: string, ingredientId: string): Promise<void>{
    await axios.delete(`${BASE_URL}/api/ingredient/${ingredientId}`, authHeaders(token))
}

export async function addAllergenToIngredient(token: string, ingredientId: string, allergenId: string): Promise<IngredientItem> {
    const current = await getIngredientById(token, ingredientId)
    const existingAllergenIds = current.allergens.map(a => a.allergenId)
    const payload = { allergenIds: [...existingAllergenIds, allergenId] }
    console.log('Adding allergen payload:', JSON.stringify(payload))
    const response = await axios.put(`${BASE_URL}/api/ingredient/${ingredientId}`, payload, authHeaders(token))
    return response.data.data
}

export async function removeAllergenFromIngredient(token: string, ingredientId: string, allergenId: string): Promise<IngredientItem>{
    const current = await getIngredientById(token, ingredientId)
    const updatedAllergenIds = current.allergens.filter(a => a.allergenId !== allergenId).map(a => a.allergenId)

    const response = await axios.put(`${BASE_URL}/api/ingredient/${ingredientId}`, {allergenIds: updatedAllergenIds}, authHeaders(token))
    return response.data.data
}