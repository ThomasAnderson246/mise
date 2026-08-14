import axios from "axios";
import { BASE_URL } from "./config";

export interface PrepListSummary{
    prepListId: string
    name: string
    createdBy: string | null
    createdByName: string | null
    totalItems: number
    completedItems: number
    isComplete: boolean
    createdAt: string

}

export interface PrepListItem {
    prepListItemId: string
    prepListId: string
    itemName: string
    quantity: number | null
    unit: string | null
    recipeId: string | null
    recipeTitle: string | null
    isComplete: boolean
    completedAt: string | null
    completedByName: string | null
    notes: string | null
}

export interface PrepList {
    prepListId: string
    tenantId: string
    name: string
    createdBy: string | null
    createdbyName: string | null
    assignedTo: string | null
    assignedToName: string | null
    totalItems: number
    completedItems: number
    isComplete: boolean
    createdAt: string
    items: PrepListItem[]
}

export interface CreatePrepListRequest {
    name: string
    assignedTo: string | null
}

export interface AddPrepListItemRequest {
    itemName: string
    quantity: number | null
    unit: string | null
    recipeId: string | null
    notes: string | null
}

export async function getPrepLists(token: string): Promise<PrepList[]>{
    const response = await axios.get(`${BASE_URL}/api/preplist`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}

export async function getPrepListById(token: string, prepListId: string): Promise<PrepList>{
    const response = await axios.get(`${BASE_URL}/api/preplist/${prepListId}`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}

export async function createPrepList(token: string, request: CreatePrepListRequest): Promise<PrepList> {
    const response = await axios.post(`${BASE_URL}/api/preplist`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}

export async function addPrepListItem(token: string, prepListId: string, request: AddPrepListItemRequest): Promise<PrepList>{
    const response = await axios.post(`${BASE_URL}/api/preplist/${prepListId}/items`, request, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function completeItem(token: string, prepListId: string, itemId: string):Promise<PrepList>{
    const response = await axios.post(`${BASE_URL}/api/preplist/${prepListId}/items/${itemId}/complete`, {}, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function forceCompleteItem(token: string, prepListId: string, itemId: string): Promise<PrepList>{
    const response = await axios.post(`${BASE_URL}/api/preplist/${prepListId}/items/${itemId}/force-complete`, {}, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function completePrepList(token: string, prepListId: string):Promise<PrepList>{
    const response = await axios.post(`${BASE_URL}/api/preplist/${prepListId}/complete`, {},{
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function forceCompletePrepList(token: string, preplistId: string): Promise<PrepList>{
    const response = await axios.post(`${BASE_URL}/api/preplist/${preplistId}/force-complete`, {}, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function assignPrepList(token: string, prepListId: string, assignedTo:string ) : Promise<PrepList>{
    const response = await axios.post(`${BASE_URL}/api/preplist/${prepListId}/assign`, {assignedTo},{
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function deletePrepListItem(token: string, prepListId: string, itemId: string ) : Promise<PrepList>{
    const response = await axios.delete(`${BASE_URL}/api/preplist/${prepListId}/items/${itemId}`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}

export async function getPrepListSummary(token: string): Promise<PrepListSummary[]>{
    const response = await axios.get(`${BASE_URL}/api/preplist/summary`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}