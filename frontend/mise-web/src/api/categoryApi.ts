import axios from "axios";
import { BASE_URL, authHeaders } from "./config";

export interface CategoryItem{
    categoryId: string
    name: string
}

export async function getCategories(token: string): Promise<CategoryItem[]>{
    const response = await axios.get(`${BASE_URL}/api/category`, authHeaders(token))
    return response.data.data
}

export async function createCategory(token: string, name: string) : Promise<CategoryItem>{
    const response = await axios.post(`${BASE_URL}/api/category`, { name }, authHeaders(token))
    return response.data.data
}

export async function deleteCategory(token: string, categoryId: string): Promise<void>{
    await axios.delete(`${BASE_URL}/api/category/${categoryId}`, authHeaders(token))
}