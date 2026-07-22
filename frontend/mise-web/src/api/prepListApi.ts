import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5282'

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

export async function getPrepListSummary(token: string): Promise<PrepListSummary[]>{
    const response = await axios.get(`${BASE_URL}/api/preplist/summary`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}