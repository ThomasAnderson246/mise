import { useState, useEffect } from "react";
import { useAuth } from "@/context/AuthContext";
import { getRecipes } from "@/api/recipeApi";
import { Button } from "../ui/button";
import type { RecipeItem } from "@/api/recipeApi";
import type { AddPrepListItemRequest } from "@/api/prepListApi";

interface AddPrepListItemFormProps {
  currentItemCount: number;
  onItemAdded: (request: AddPrepListItemRequest) => void;
  onCancel: () => void;
}

export function AddPrepListItemForm({
  currentItemCount,
  onItemAdded,
  onCancel,
}: AddPrepListItemFormProps) {
  const { user } = useAuth();

  const [sourceType, setSourceType] = useState<"recipe" | "portion" | "custom">(
    "recipe",
  );
  const [recipes, setRecipes] = useState<RecipeItem[]>([]);
  const [portions, setPortions] = useState<RecipeItem[]>([]);
  const [loadingRecipes, setLoadingRecipes] = useState(true);

  //recipe or portion fields
  const [selectedRecipeId, setSelectedRecipeId] = useState("");
  const [scalingFactor, setScalingFactor] = useState("1");
  const [anchorIngredientId, setAnchorIngredientId] = useState("");
  const [anchorQuantity, setAnchorQuantity] = useState("");

  //custom fields
  const [itemName, setItemName] = useState("");
  const [quantity, setQuantity] = useState("");
  const [unit, setUnit] = useState("");
  const [notes, setNotes] = useState("");

  //recipe data for anchor detection
  const [selectedRecipe, setSelectedRecipe] = useState<RecipeItem | null>(null);

  useEffect(() => {
    if (!user?.token) return;

    getRecipes(user.token)
      .then((data) => {
        setRecipes(
          data.filter((r) => !r.isPortion && r.status === "published"),
        );
        setPortions(
          data.filter((r) => r.isPortion && r.status === "published"),
        );
      })
      .catch(() => {})
      .finally(() => setLoadingRecipes(false));
  }, [user]);

  function handleRecipeSelect(recipeId: string) {
    setSelectedRecipeId(recipeId);
    const recipe = [...recipes, ...portions].find(
      (r) => r.recipeId === recipeId,
    );
    setSelectedRecipe(recipe ?? null);
    // reset scaling fields
    setScalingFactor("1");
    setAnchorIngredientId("");
    setAnchorQuantity("");
  }

  function handleSubmit() {
    const isRatioMode = selectedRecipe?.scalingMode === "ratio";

    let request: AddPrepListItemRequest;

    if (sourceType === "custom") {
      if (!itemName.trim()) return;
      request = {
        sourceType: "custom",
        itemName,
        recipeId: null,
        scalingFactor: null,
        anchorIngredientId: null,
        anchorQuantity: null,
        quantity: quantity ? parseFloat(quantity) : null,
        unit: unit || null,
        notes: notes || null,
        displayOrder: currentItemCount + 1,
      };
    } else {
      if (!selectedRecipe) return;
      const recipe = [...recipes, ...portions].find(
        (r) => r.recipeId === selectedRecipeId,
      );
      request = {
        sourceType,
        itemName: recipe?.title ?? "",
        recipeId: selectedRecipeId,
        scalingFactor: isRatioMode ? null : parseFloat(scalingFactor) || 1,
        anchorIngredientId:
          isRatioMode && anchorIngredientId ? anchorIngredientId : null,
        anchorQuantity:
          isRatioMode && anchorQuantity ? parseFloat(anchorQuantity) : null,
        quantity: null,
        unit: null,
        notes: notes || null,
        displayOrder: currentItemCount + 1,
      };
    }
    onItemAdded(request);
  }

  return (
    <div className="p-4 bg-card rounded-lg border border-border space-y-4">
      <p className="text-sm font-medium text-foreground">Add prep item</p>

      <div className="flex gap-2">
        {(["recipe", "portion", "custom"] as const).map((type) => (
          <button
            key={type}
            onClick={() => {
              setSourceType(type);
              setSelectedRecipeId("");
              setSelectedRecipe(null);
              setItemName("");
            }}
            className={`text-sm px-4 py-2 rounded-lg border transition-colors ${
              sourceType === type
                ? "bg-primary text-primary-foreground border-primary"
                : "bg-background text-foreground border-border hover:border-primary"
            }`}
          >
            {type.charAt(0).toUpperCase() + type.slice(1)}
          </button>
        ))}
      </div>

      {sourceType === "recipe" && (
        <div className="space-y-3">
          <select
            value={selectedRecipeId}
            onChange={(e) => handleRecipeSelect(e.target.value)}
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            <option value="">Select a recipe...</option>
            {loadingRecipes ? (
              <option disabled>Loading...</option>
            ) : (
              recipes.map((r) => (
                <option key={r.recipeId} value={r.recipeId}>
                  {r.title}
                </option>
              ))
            )}
          </select>

          {selectedRecipe && selectedRecipe.scalingMode === "multiplier" && (
            <div className="flex items-center gap-3">
              <label className="text-sm text-muted-foreground flex-shrink-0">
                Batches:
              </label>
              <input
                type="number"
                min="0.25"
                step="0.25"
                value={scalingFactor}
                onChange={(e) => setScalingFactor(e.target.value)}
                className="w-24 px-4 py-2 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
              />
              <span className="text-sm text-muted-foreground">
                x base recipes
              </span>
            </div>
          )}

          {selectedRecipe && selectedRecipe.scalingMode === "ratio" && (
            <div className="space-y-2">
              <p className="text-xs text-muted-foreground">
                This recipe uses ratio scaling. Enter the quantity of your
                anchor ingredient.
              </p>
              <div className="flex items-center gap-3">
                <label className="text-sm text-muted-foreground flex-shrink-0">
                  Anchor quantity:{" "}
                </label>
                <input
                  type="number"
                  min="0"
                  step="0.1"
                  value={anchorQuantity}
                  onChange={(e) => setAnchorQuantity(e.target.value)}
                  placeholder="Amount"
                  className="w-24 px-4 py-2 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
                />
              </div>
            </div>
          )}
        </div>
      )}

      {sourceType === "portion" && (
        <div className="space-y-3">
          <select
            value={selectedRecipeId}
            onChange={(e) => handleRecipeSelect(e.target.value)}
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            <option value="">Selected a portion...</option>
            {loadingRecipes ? (
              <option disabled> Loading...</option>
            ) : (
              portions.map((p) => (
                <option key={p.recipeId} value={p.recipeId}>
                  {p.title}
                </option>
              ))
            )}
          </select>

          {selectedRecipe && (
            <div className="flex items-center gap-3">
              <label className="text-sm text-muted-foregorund flex-shrink-0">
                Portions:
              </label>
              <input
                type="number"
                min="1"
                step="1"
                value={scalingFactor}
                onChange={(e) => setScalingFactor(e.target.value)}
                className="w-24 px-4 py-2 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
              />
              <span className="text-sm text-muted-foreground">portions</span>
            </div>
          )}
        </div>
      )}

      {sourceType === "custom" && (
        <div className="space-y-3">
          <input
            type="text"
            value={itemName}
            onChange={(e) => setItemName(e.target.value)}
            placeholder="Item name..."
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
            autoFocus
          />
          <div className="flex gap-2">
            <input
              type="number"
              value={quantity}
              onChange={(e) => setQuantity(e.target.value)}
              placeholder="Qty"
              className="w-24 px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
            />
            <input
              type="text"
              value={unit}
              onChange={(e) => setUnit(e.target.value)}
              placeholder="Unit"
              className="w-28 px-4 py-2.5 rounded-lg border border-border bg-background text-sm focus:outline-none focus:ring-2 focus:ring-ring"
            />
          </div>
        </div>
      )}

      {/* notes - visible for all source types*/}
      <input
        type="text"
        value={notes}
        onChange={(e) => setNotes(e.target.value)}
        placeholder="Notes (optional)"
        className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
      />

      <div className="flex gap-2">
        <Button
          onClick={handleSubmit}
          disabled={
            (sourceType === "custom" && !itemName.trim()) ||
            (sourceType !== "custom" && !selectedRecipeId)
          }
          className="bg-primary text-primary-foreground"
        >
          Add
        </Button>
        <Button variant="outline" onClick={onCancel}>
          Cancel
        </Button>
      </div>
    </div>
  );
}
