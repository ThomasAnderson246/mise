import axios from 'axios'
import { BASE_URL } from './config'

export interface UserItem {
    userId: string
    email: string
    firstName: string
    lastName: string
    role: string
    status: string
}

export async function getUsers(token: string): Promise<UserItem[]>{
    const response = await axios.get(`${BASE_URL}/api/user`, {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}