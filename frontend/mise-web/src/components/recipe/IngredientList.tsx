import type { RecipeIngredient } from "@/api/recipeApi";

interface IngredientListProps {
  ingredients: RecipeIngredient[];
  onRemove: (recipeIngredientId: string) => void;
}

export function IngredientList({ ingredients, onRemove }: IngredientListProps) {
  if (ingredients.length === 0) return null;

  return (
    <ul className="space-y-2 mb-4">
      {ingredients.map((ing) => (
        <li
          key={ing.recipeIngredientId}
          className="flex items-center gap-3 bg-card rounded-lg px-4 py-2.5 border border-border"
        >
          <span className="w-20 text-right text-sm font-medium text-foreground flex-shrink-0">
            {ing.quantity} {ing.unitName ?? ""}
          </span>
          <span className="flex-1 text-sm text-foreground">
            {" "}
            {ing.ingredientName}
          </span>
          <button
            onClick={() => onRemove(ing.recipeIngredientId)}
            className="text-xs text-destructive hover:underline flex-shrink-0"
          >
            Remove
          </button>
        </li>
      ))}
    </ul>
  );
}
