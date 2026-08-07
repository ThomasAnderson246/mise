import type { RecipeIngredient } from "@/api/recipeApi";

interface ScalingControlProps {
  isRatioMode: boolean;
  scalingFactor: number;
  onScalingFactorChange: (factor: number) => void;
  anchorIngredient: RecipeIngredient | null;
  anchorQuantity: number | null;
  onAnchorQuantityChange: (quantity: number) => void;
}

export function ScalingControl({
  isRatioMode,
  scalingFactor,
  onScalingFactorChange,
  anchorIngredient,
  anchorQuantity,
  onAnchorQuantityChange,
}: ScalingControlProps) {
  if (isRatioMode && anchorIngredient) {
    return (
      <div className="flex items-center gap-3 mb-4 p-3 bg-card rounded-lg border border-border">
        <span className="text-sm text-muted-foreground flex-shrink-0">
          Scale by {anchorIngredient.ingredientName}
        </span>
        <input
          type="number"
          min="0.1"
          step="0.1"
          value={anchorQuantity ?? anchorIngredient.quantity}
          onChange={(e) => onAnchorQuantityChange(parseFloat(e.target.value))}
          className="w-24 px-3 py-1.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
        />
        <span className="text-sm text-muted-foreground">
          {anchorIngredient.unitName ?? ""}
        </span>
        <button
          onClick={() => onAnchorQuantityChange(anchorIngredient.quantity)}
          className="text-xs text-secondary hover:underline ml-auto"
        >
          Reset
        </button>
      </div>
    );
  }

  return (
    <div className="flex items-center gap-3 mb-4 p-3 bg-card rounded-lg border border-border">
      <span className="text-sm text-muted-foreground flex-shrink-0">
        Scale:
      </span>
      <button
        onClick={() =>
          onScalingFactorChange(Math.max(0.25, scalingFactor - 0.25))
        }
        className="w-7 h-7 rounded-full border border-border text-foreground hover:border-primary transition-colors text-sm"
      >
        {" "}
        -
      </button>
      <input
        type="number"
        min="0.25"
        step="0.25"
        value={scalingFactor}
        onChange={(e) => onScalingFactorChange(parseFloat(e.target.value) || 1)}
        className="w-20 px-3 py-1.5 rounded-lg border border-border bg-background text-foreground text-sm text-center focus:outline-none focus:ring-2 focus:ring-ring"
      />
      <button
        onClick={() => onScalingFactorChange(scalingFactor + 0.25)}
        className="w-7 h-7 rounded-full border border-border text-foreground hover:border-primary transition-colors text-sm"
      >
        +
      </button>
      <span className="text-sm text-muted-foreground">x</span>
      <button
        onClick={() => onScalingFactorChange(1)}
        className="text-xs text-secondary hover:underline ml-auto"
      >
        Reset
      </button>
    </div>
  );
}
