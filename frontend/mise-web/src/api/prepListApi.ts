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

export async function getPrepListSummary(token: string): Promise<PrepListSummary[]>{
    const response = await axios.get(`${BASE_URL}/api/preplist/summary`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}