import axios from "axios";
import { BASE_URL } from "./config";

export interface IngredientItem{
    ingredientId: string
    name: string
    defaultUnitTypeId: string | null
    defaultUnitTypeName: string | null
    isNonConvertible: boolean
}

export async function getIngredients(token: string): Promise<IngredientItem[]>{
    const response = await axios.get(`${BASE_URL}/api/ingredient`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}

export async function searchIngredients(token: string, term: string): Promise<IngredientItem[]>{
    const response = await axios.get(`${BASE_URL}/api/ingredient/search?term=${encodeURIComponent(term)}`, {
        withCredentials: true,
        headers:{ Authorization: `Bearer ${token}`}
    })

    return response.data.data
}

export async function createIngredient(token: string, request: {
    name: string
    category: string | null
    defaultUnitTypeId: string | null
    isNonConvertible: boolean
    allergenIds: string[]
}): Promise<IngredientItem> {
    const response = await axios.post(`${BASE_URL}/api/ingredient`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}