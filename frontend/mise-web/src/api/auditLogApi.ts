import axios from "axios";
import { BASE_URL, authHeaders } from "./config";

export interface AuditLogEntry {
    auditLogId: string
    tenantId: string
    performedBy: string | null
    performedByName: string | null
    action: string
    resource: string
    resourceId: string
    previousState: string | null
    newState: string | null
    ipAddress: string | null
    performedAt: string
}

export interface AuditLogFilters {
    entityType?: string
    actions?: string[]
    fromDate?: string
    toDate?: string
    performedBy?: string
}

export async function getAuditLogs(token: string): Promise<AuditLogEntry[]>{
    const response = await axios.get(`${BASE_URL}/api/auditlog`, authHeaders(token))
    return response.data.data
}


// search param will be added post-alpha
/*export async function getAuditLogs(token: string, filters?: AuditLogFilters): Promise<AuditLogEntry[]>{
    const params = new URLSearchParams()
    if (filters?.entityType) params.append('entityType', filters.entityType)
    if (filters?.actions?.length) filters.actions.forEach(a => params.append('actions', a))
    if (filters?.fromDate) params.append('fromDate', filters.fromDate)
    if (filters?.toDate) params.append('toDate', filters.toDate)
    if (filters?.performedBy) params.append('performedby', filters.performedBy)

        const url = `${BASE_URL}/api/auditlog${params.toString() ? '?' + params.toString() : ''}`
        const response = await axios.get(url, authHeaders(token))
        return response.data.data
}*/