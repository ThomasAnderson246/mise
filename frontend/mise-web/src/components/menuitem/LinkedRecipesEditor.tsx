import { useState } from "react";
import { Button } from "../ui/button";
import { inputClass, selectClass } from "@/lib/styles";
import type { MenuItemRecipe } from "@/api/menuItemApi";
import type { RecipeItem } from "@/api/recipeApi";

interface LinkedRecipesEditorProps {
  recipes: MenuItemRecipe[];
  availableRecipes: RecipeItem[];
  onAdd: (recipeId: string, note: string | null) => Promise<void>;
  onRemove: (recipeId: string) => Promise<void>;
}

export function LinkedRecipesEditor({
  recipes,
  availableRecipes,
  onAdd,
  onRemove,
}: LinkedRecipesEditorProps) {
  const [showAddRecipe, setShowAddRecipe] = useState(false);
  const [selectedRecipeId, setSelectedRecipeId] = useState("");
  const [recipeNote, setRecipeNote] = useState("");
  const [adding, setAdding] = useState(false);

  async function handleAdd() {
    if (!selectedRecipeId) return;
    setAdding(true);
    try {
      await onAdd(selectedRecipeId, recipeNote || null);
      setSelectedRecipeId("");
      setRecipeNote("");
      setShowAddRecipe(false);
    } finally {
      setAdding(false);
    }
  }

  return (
    <div className="mb-8">
      <h2 className="text-lg font-semibold text-foreground mb-4 pb-2 border-b border-border ">
        Linked Recipes
      </h2>

      {recipes.length === 0 && (
        <p className="text-sm text-muted-foreground mb-4">
          No recipes linked yet.
        </p>
      )}

      {recipes.map((r) => (
        <div
          key={r.menuItemRecipeId}
          className="flex items-center gap-3 p-3 bg-card rounded-lg border border-border"
        >
          <div className="flex-1">
            <p className="text-sm font-medium text-foreground">
              {r.recipeTitle}
            </p>
            {r.note && (
              <p className="text-xs text-muted-foreground mt-0.5">{r.note}</p>
            )}
          </div>
          <button
            onClick={() => onRemove(r.recipeId)}
            className="text-xs text-destructive hover:underline"
          >
            Remove
          </button>
        </div>
      ))}

      {!showAddRecipe ? (
        <Button
          variant="outline"
          onClick={() => setShowAddRecipe(true)}
          className="w-full"
        >
          + Link recipe
        </Button>
      ) : (
        <div className="p-4 bg-card rounded-lg border border-border space-y-3">
          <select
            value={selectedRecipeId}
            onChange={(e) => setSelectedRecipeId(e.target.value)}
            className={selectClass}
          >
            <option value="">Select a recipe...</option>
            {availableRecipes.map((r) => (
              <option key={r.recipeId} value={r.recipeId}>
                {r.title}
              </option>
            ))}
          </select>
          <input
            type="text"
            value={recipeNote}
            onChange={(e) => setRecipeNote(e.target.value)}
            placeholder="Note (optional)"
            className={inputClass}
          />
          <div className="flex gap-2">
            <Button
              onClick={handleAdd}
              disabled={adding || !selectedRecipeId}
              className="bg-primary text-primary-foreground"
            >
              {adding ? "Linking..." : "Link"}
            </Button>
            <Button
              variant="outline"
              onClick={() => {
                setShowAddRecipe(false);
                setSelectedRecipeId("");
                setRecipeNote("");
              }}
            >
              Cancel
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}
