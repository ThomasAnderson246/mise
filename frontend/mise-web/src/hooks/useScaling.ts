import { useState, useMemo } from "react";
import type { RecipeVersion, RecipeIngredient } from "@/api/recipeApi";

export function useScaling(version: RecipeVersion | null, scalingMode: string) {
    const [scalingFactor, setScalingFactor] = useState(1)
    const [anchorQuantity, setAnchorQuantity] = useState<number | null>(null)

    const anchorIngredient = useMemo(() => {
        if (!version) return null
        const allIngredients = [
            ...version.ingredients,
            ...version.recipeIngredientGroups.flatMap(g => g.ingredients)
        ]
        return allIngredients.find(i => i.isRatioAnchor) ?? null
    }, [version])

    const isRatioMode = scalingMode === 'ratio'

    function getScaledQuantity(ingredient: RecipeIngredient): number {
        if (!version) return ingredient.quantity

        let scaled: number

        if (isRatioMode && anchorIngredient) {
            const effectiveAnchor = anchorQuantity ?? anchorIngredient.quantity
            const ratio = ingredient.quantity / anchorIngredient.quantity
            scaled = ratio * effectiveAnchor
        } else {
            scaled = ingredient.quantity * scalingFactor
        }

        if (ingredient.measureType === "count") {
            return Math.ceil(scaled)
        }

        return Math.round(scaled * 1000) / 1000
    }

    function formatQuantity(quantity: number): string{
        if (Number.isInteger(quantity)) return quantity.toString()
            return quantity.toFixed(2).replace(/\.?0+$/,'')
    }

    return {
        scalingFactor,
        setScalingFactor,
        anchorQuantity,
        setAnchorQuantity,
        anchorIngredient,
        isRatioMode,
        getScaledQuantity,
        formatQuantity,

    }
}