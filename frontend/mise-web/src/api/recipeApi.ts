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

export interface RecipeStep{
    stepId: string
    stepNumber: number
    instruction: string
    durationMinutes: number | null
}

export interface RecipeIngredient {
    recipeIngredientId: string
    ingredientName: string
    quantity :number
    unitName: string | null
    notes: string | null
}

export interface RecipeDetail {
    recipeId: string
    title: string 
    description: string
    status: string
    scalingMode: string
    tenantId: string
    createdAt: string
    updatedAt: string
    currentVersion: {
        versionId: string
        versionNumber: number
        isDraft: boolean
        isPublished: boolean
        steps: RecipeStep[]
        ingredients: RecipeIngredient[]
    } | null
    recipeCategories: {
        category: {
            categoryId: string
            name: string
        }
    }[]
}

export async function getRecipes(token: string): Promise<RecipeItem[]>{
    const response = await axios.get(`${BASE_URL}/api/recipe`,{
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function getRecipeById(token: string, recipeId: string): Promise<RecipeDetail>{
    const response = await axios.get(`${BASE_URL}/api/recipe/${recipeId}`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}

export interface SubRecipeItem{
    parentRecipeId: string
    subRecipeId: string
    subRecipeTitle: string
    subRecipeStatus: string
}

export async function getSubRecipes(token: string, recipeId: string): Promise<SubRecipeItem[]>{
    const response = await axios.get(`${BASE_URL}/api/recipe/${recipeId}/subrecipes`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}