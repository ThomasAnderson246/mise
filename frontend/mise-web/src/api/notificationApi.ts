import axios from "axios";
import { BASE_URL, authHeaders } from "./config";

export interface NotificationItem{
    notificationId: string
    title: string
    message: string
    type: string
    isRead: boolean
    createdAt: string
}

export async function getUnreadNotifications(token: string): Promise<NotificationItem[]>{
    const response = await axios.get(`${BASE_URL}/api/notification/unread`,authHeaders(token))
    return response.data.data
}

export async function getAllNotifications(token:string): Promise<NotificationItem[]>{
    const response = await axios.get(`${BASE_URL}/api/notification`, authHeaders(token))

    return response.data.data
}

export async function markAsRead(token:string, notificationId: string): Promise<void>{
    await axios.post(`${BASE_URL}/api/notification/${notificationId}/read`, {}, authHeaders(token))
}

export async function markAllAsRead(token: string): Promise<void>{
    await axios.post(`${BASE_URL}/api/notification/read-all`, {}, authHeaders(token))
}

export async function sendDirectMessage(token:string, recipientId: string, message: string) : Promise<void>{
    await axios.post(`${BASE_URL}/api/notification/direct`, {recipientId, message}, authHeaders(token))
}

export async function sendSystemMessage(token: string, title: string, message: string): Promise<void>{
    await axios.post(`${BASE_URL}/api/notification/system`, {title, message}, authHeaders(token))
}