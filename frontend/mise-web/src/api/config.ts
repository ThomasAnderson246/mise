

export const BASE_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5282'

export function authHeaders(token: string) {
    return {
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    }
}