import axios from "axios";
import { BASE_URL } from "./config";

export interface CreateRecipeRequest{
    title: string
    description: string | null
    scalingMode: string
    categoryIds: string[]
}

export interface UpdateRecipeRequest {
    title?: string
    description?: string | null
    scalingMode?: string
    categoryIds?: string[]
}

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

export async function createRecipe(token: string, request: CreateRecipeRequest): Promise<RecipeDetail>{
    const response = await axios.post(`${BASE_URL}/api/recipe`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function updateRecipe(token: string, recipeId: string, request: UpdateRecipeRequest): Promise<RecipeDetail>{
    const response = await axios.put(`${BASE_URL}/api/recipe/${recipeId}`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}

export async function addIngredient(token: string, recipeId: string, request:{
    ingredientId: string
    quantity: number
    unitTypeId: string | null
    displayOrder: number
    groupId: string | null
    isNonConvertible: boolean
    isRatioAnchor: boolean
}) : Promise<void> {
    await axios.post(`${BASE_URL}/api/recipe/${recipeId}/ingredients`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
}

export async function removeIngredient(token: string, recipeId: string, recipeIngredientId: string): Promise<void>{
    await axios.delete(`${BASE_URL}/api/recipe/${recipeId}/ingredients/${recipeIngredientId}`,{
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
}

export async function addStep(token: string, recipeId: string, request:{
    stepNumber: number
    instruction: string
    hasTimer: boolean
    timerDuration: number | null
    isAsync: boolean
    asyncGroupId: string | null
}) : Promise<void> {
    await axios.post(`${BASE_URL}/api/recipe/${recipeId}/steps`, request, {
        withCredentials:true,
        headers: {Authorization: `Bearer ${token}`}
    })
}

export async function removeStep(token: string, recipeId: string, stepId: string): Promise<void>{
    await axios.delete(`${BASE_URL}/api/recipe/${recipeId}/steps/${stepId}`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
}

export async function publishRecipe(token: string, recipeId:string):Promise<void>{
    await axios.post(`${BASE_URL}/api/recipe/${recipeId}/publish`, {},{
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
}