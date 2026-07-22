import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5282'

export interface RecipeItem{
    recipeId: string
    title: string
    description: string | null
    status: string
    scalingMode: string
    tenantId: string
    createdAt:string
    updatedAt: string
}

export async function getRecipes(token: string): Promise<RecipeItem[]>{
    const response = await axios.get(`${BASE_URL}/api/recipe`,{
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}