import axios from "axios";
import { BASE_URL, authHeaders } from "./config";
import type { UnitSystem, MeasureType } from "@/types";

export interface UnitTypeItem{
    unitTypeId: string
    name: string
    abbreviation: string
    measureType: MeasureType
    system: UnitSystem
}

export interface CreateUnitTypeRequest {
    name: string
    abbreviation: string
    measureType: MeasureType
    system: UnitSystem
}

export async function getUnitTypes(token: string): Promise<UnitTypeItem[]>{
    const response = await axios.get(`${BASE_URL}/api/unittype`, authHeaders(token))
    return response.data.data
}

export async function createUnitType(token: string, request: CreateUnitTypeRequest) : Promise<UnitTypeItem> {
    const response = await axios.post(`${BASE_URL}/api/unittype`, request, authHeaders(token))
    return response.data.data
}

export async function deleteUnitType(token: string, unitTypeId:string): Promise<void>{
    await axios.delete(`${BASE_URL}/api/unittype/${unitTypeId}`, authHeaders(token))
}