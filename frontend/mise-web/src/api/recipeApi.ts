import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5282'

export interface RecipeIngredientGroup {
    groupId: string
    name: string
    displayOrder: number
    ingredients: RecipeIngredient[]
}

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
    hasTimer: boolean
    timerDuration: number | null
    isAsync: boolean
    asyncGroupId: string | null
    
}

export interface RecipeVersion{
    versionId: string
    versionNumber: number
    isDraft: boolean
    isPublished: boolean
    recipeIngredientGroups: RecipeIngredientGroup[]
    ingredients: RecipeIngredient[]
    steps: RecipeStep[]
}

export interface RecipeIngredient {
    recipeIngredientId: string
    ingredientName: string
    quantity :number
    unitName: string | null
    displayOrder: number
    groupId: string | null
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
    recipeCategories: {
        categoryId: string
        name: string
    }[]
    currentVersion: RecipeVersion | null
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