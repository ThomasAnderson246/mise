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

        if (isRatioMode && anchorIngredient) {
            const effectiveAnchor = anchorQuantity ?? anchorIngredient.quantity
            const ratio = ingredient.quantity / anchorIngredient.quantity
            return Math.round((ratio * effectiveAnchor) * 1000) / 1000
        }

        return Math.round((ingredient.quantity * scalingFactor) * 1000) /1000
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