import axios from "axios";

const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5282'

export interface NotificationItem{
    notificationId: string
    title: string
    message: string
    type: string
    isRead: boolean
    createdAt: string
}

export async function getUnreadNotifications(token: string): Promise<NotificationItem[]>{
    const response = await axios.get(`${BASE_URL}/api/notification/unread`,{
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })
    return response.data.data
}