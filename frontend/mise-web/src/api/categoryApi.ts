import axios from "axios";
import { BASE_URL } from "./config";

export interface CategoryItem{
    categoryId: string
    name: string
}

export async function getCategories(token: string): Promise<CategoryItem[]>{
    const response = await axios.get(`${BASE_URL}/api/category`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}