import { useState } from "react";
import { Button } from "../ui/button";
import type { RecipeIngredient } from "@/api/recipeApi";
import type { UnitTypeItem } from "@/api/unitTypeApi";

interface IngredientListProps {
  ingredients: RecipeIngredient[];
  unitTypes: UnitTypeItem[];
  onRemove: (recipeIngredientId: string) => void;
  onUpdate: (
    recipeIngredientId: string,
    quantity: number,
    unitTypeId: string | null,
  ) => void;
}

export function IngredientList({
  ingredients,
  unitTypes,
  onRemove,
  onUpdate,
}: IngredientListProps) {
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editQuantity, setEditQuantity] = useState("");
  const [editUnitTypeId, setEditUnitTypeId] = useState("");

  function startEdit(ing: RecipeIngredient) {
    console.log("startEdit called for:", ing.recipeIngredientId);
    setEditingId(ing.recipeIngredientId);
    setEditQuantity(ing.quantity.toString());
    setEditUnitTypeId(ing.unitTypeId ?? "");
  }

  function handleSave(recipeIngredientId: string) {
    if (!editQuantity) return;
    onUpdate(
      recipeIngredientId,
      parseFloat(editQuantity),
      editUnitTypeId || null,
    );
    setEditingId(null);
  }

  function handleCancel() {
    setEditingId(null);
    setEditQuantity("");
    setEditUnitTypeId("");
  }

  if (ingredients.length === 0) return null;

  return (
    <ul className="space-y-2 mb-4">
      {ingredients.map((ing) => (
        <li
          key={ing.recipeIngredientId}
          className="bg-card rounded-lg border border-border overflow-hidden"
        >
          {editingId === ing.recipeIngredientId ? (
            // edit mode
            <div className="flex items-center gap-2 px-4 py-2.5">
              <span className="text-sm text-foreground flex-shrink-0 font-medium">
                {ing.ingredientName}
              </span>
              <input
                type="number"
                value={editQuantity}
                onChange={(e) => setEditQuantity(e.target.value)}
                className="w-20 px-2 py-1 rounded border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
                autoFocus
              />
              <select
                value={editUnitTypeId}
                onChange={(e) => setEditUnitTypeId(e.target.value)}
                className="flex-1 px-2 py-1 rounded border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
              >
                <option value="">No unit</option>
                {unitTypes.map((ut) => (
                  <option key={ut.unitTypeId} value={ut.unitTypeId}>
                    {ut.name} ({ut.abbreviation})
                  </option>
                ))}
              </select>
              <Button
                onClick={() => handleSave(ing.recipeIngredientId)}
                disabled={!editQuantity}
                className="bg-primary text-primary-foreground text-xs h-8 px-3 flex-shrink-0"
              >
                Save
              </Button>
              <button
                onClick={handleCancel}
                className="text-xs text-muted-foreground hover:text-foreground flex-shrink-0"
              >
                Cancel
              </button>
            </div>
          ) : (
            //Display mode
            <div className="flex items-center gap-3 px-4 py-2.5">
              <span className="w-20 text-right text-sm font-medium text-foreground flex-shrink-0">
                {ing.quantity} {ing.unitName ?? ""}
              </span>
              <span className="flex-1 text-sm text-foreground">
                {ing.ingredientName}
              </span>
              <button
                onClick={() => startEdit(ing)}
                className="text-xs text-secondary hover:underline flex-shrink-0"
              >
                Edit
              </button>
              <button
                onClick={() => onRemove(ing.recipeIngredientId)}
                className="text-xs text-destructive hover:underline flex-shrink-0"
              >
                Remove
              </button>
            </div>
          )}
        </li>
      ))}
    </ul>
  );
}
