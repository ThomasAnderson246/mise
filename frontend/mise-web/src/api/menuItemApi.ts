import axios from "axios";
import { BASE_URL } from "./config";

export interface MenuItemAllergen {
    menuItemAllergenId: string
    allergenId: string
    allergenName: string
    isMajor: boolean
    sourceName: string
    sourceComponent: string | null
    isDirect: boolean
    isManual: boolean
}

export interface MenuItemRecipe {
    menuItemRecipeId: string
    recipeId: string
    recipeTitle: string
    recipeStatus: string
    displayOrder: number
    note: string | null
}

export interface MenuItem {
    menuItemId: string
    tenantId: string
    name: string
    description: string | null
    course: string | null
    status: string
    isActive: boolean   
    createdBy: string | null
    createdByName: string | null
    createdAt: string
    updatedAt: string
    recipes: MenuItemRecipe[]
    allergens: MenuItemAllergen[]
}

export interface CreateMenuItemRequest {
    name: string
    description: string | null
    course: string | null
}

export interface AddMenuItemRecipeRequest {
    recipeId: string
    displayOrder: number
    note: string | null
}

export interface AddMenuItemAllergenRequest {
    allergenId: string
    sourceName: string
    sourceComponent: string | null
}

export async function getMenuItems(token: string): Promise<MenuItem[]> {
    const response = await axios.get(`${BASE_URL}/api/menuitem`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function getMenuItemById(token: string, menuItemId:string): Promise<MenuItem> {
    const response = await axios.get(`${BASE_URL}/api/menuitem/${menuItemId}`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    
    return response.data.data
}

export async function createMenuItem(token: string, request: CreateMenuItemRequest): Promise<MenuItem>{
    const response = await axios.post(`${BASE_URL}/api/menuitem`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function updateMenuItem(token: string, menuItemId: string, request: Partial<CreateMenuItemRequest>): Promise<MenuItem>{
    const response = await axios.put(`${BASE_URL}/api/menuitem/${menuItemId}`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function publishMenuItem(token: string, menuItemId: string): Promise<MenuItem>{
    const response = await axios.post(`${BASE_URL}/api/menuitem/${menuItemId}/publish`, {}, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function AddMenuItemRecipe(token: string, menuItemId: string, request: AddMenuItemRecipeRequest): Promise<MenuItem>{
    const response = await axios.post(`${BASE_URL}/api/menuitem/${menuItemId}/recipes`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function removeMenuItemRecipe(token:string, menuItemId: string, recipeId: string): Promise<MenuItem>{
    const response = await axios.delete(`${BASE_URL}/api/menuitem/${menuItemId}/recipes/${recipeId}`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function resolveAllergens(token: string, menuiTemId: string): Promise<MenuItem>{
    const response = await axios.post(`${BASE_URL}/api/menuitem/${menuiTemId}/resolve-allergens`, {}, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function addManualAllergen(token: string, menuItemId: string, request: AddMenuItemAllergenRequest): Promise<MenuItem>{
    const response = await axios.post(`${BASE_URL}/api/menuitem/${menuItemId}/allergens`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function removeManualAllergen(token: string, menuItemId: string, allergenId: string): Promise<MenuItem>{
    const response = await axios.delete(`${BASE_URL}/api/menuitem/${menuItemId}/allergens/${allergenId}`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}