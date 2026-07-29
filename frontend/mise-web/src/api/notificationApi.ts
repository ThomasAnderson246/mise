import axios from "axios";
import { BASE_URL } from "./config";

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