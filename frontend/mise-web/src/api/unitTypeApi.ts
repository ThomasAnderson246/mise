import axios from "axios";
import { BASE_URL } from "./config";
import type { UnitSystem, MeasureType } from "@/types";

export interface UnitTypeItem{
    unitTypeId: string
    name: string
    abbreviation: string
    measureType: MeasureType
    system: UnitSystem
}

export async function getUnitTypes(token: string): Promise<UnitTypeItem[]>{
    const response = await axios.get(`${BASE_URL}/api/unittype`,{
        withCredentials: true,
        headers: {Authorization: `Bearer ${token}`}
    })

    return response.data.data
}