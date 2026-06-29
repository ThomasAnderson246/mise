import axios from 'axios'

const BASE_URL = import.meta.env.VITE_API_URL ?? 'https://localhost:7144'

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