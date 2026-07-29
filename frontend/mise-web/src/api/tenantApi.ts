import axios from 'axios'
import { BASE_URL } from './config'

export interface TenantResponse {
    tenantId: string
    name: string
    slug: string
    logoUrl: string | null
    primaryColour: string | null
    secondaryColour: string | null
}

export async function getTenantBySlug(slug: string): Promise<TenantResponse>{
    const response = await axios.get(`${BASE_URL}/api/tenant/${slug}`)
    return response.data.data
}