import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { searchIngredients, createIngredient } from "@/api/ingredientApi";
import { Button } from "../ui/button";
import { toast } from "sonner";
import axios from "axios";
import type { RecipeIngredient } from "@/api/recipeApi";
import type { IngredientItem } from "@/api/ingredientApi";
import type { UnitTypeItem } from "@/api/unitTypeApi";

interface IngredientSearchProps {
  unitTypes: UnitTypeItem[];
  currentIngredientCount: number;
  onIngredientAdded: (ingredient: RecipeIngredient) => void;
}

export function IngredientSearch({
  unitTypes,
  currentIngredientCount,
  onIngredientAdded,
}: IngredientSearchProps) {
  const { user } = useAuth();

  const [ingredientSearch, setIngredientSearch] = useState("");
  const [ingredientResults, setIngredientResults] = useState<IngredientItem[]>(
    [],
  );
  const [showDropdown, setShowDropdown] = useState(false);
  const [selectedIngredient, setSelectedIngredient] =
    useState<IngredientItem | null>(null);
  const [quantity, setQuantity] = useState("");
  const [unitTypeId, setUnitTypeId] = useState("");

  const [showNewIngredientForm, setShowNewIngredientForm] = useState(false);
  const [newName, setNewName] = useState("");
  const [newCategory, setNewCategory] = useState("");
  const [newUnitTypeId, setNewUnitTypeId] = useState("");
  const [newIsNonConvertible, setNewIsNonConvertible] = useState(false);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    if (!user?.token || ingredientSearch.length < 2) {
      setIngredientResults([]);
      setShowDropdown(false);
      return;
    }

    const timer = setTimeout(async () => {
      try {
        const results = await searchIngredients(user.token, ingredientSearch);
        setIngredientResults(results);
        setShowDropdown(true);
      } catch {
        setIngredientResults([]);
      }
    }, 300);

    return () => clearTimeout(timer);
  }, [ingredientSearch, user]);

  function handleSelectedIngredient(ing: IngredientItem) {
    setSelectedIngredient(ing);
    setIngredientSearch(ing.name);
    setUnitTypeId(ing.defaultUnitTypeId ?? "");
    setShowDropdown(false);
  }

  function handleAdd() {
    if (!selectedIngredient || !quantity) return;

    const newIngredient: RecipeIngredient = {
      recipeIngredientId: crypto.randomUUID(),
      ingredientName: selectedIngredient.name,
      ingredientId: selectedIngredient.ingredientId,
      quantity: parseFloat(quantity),
      unitName:
        unitTypes.find((u) => u.unitTypeId === unitTypeId)?.name ?? null,
      unitTypeId: unitTypeId || null,
      displayOrder: currentIngredientCount + 1,
      groupId: null,
    };

    onIngredientAdded(newIngredient);
    setSelectedIngredient(null);
    setIngredientSearch("");
    setQuantity("");
    setUnitTypeId("");
  }

  async function handleCreateIngredient() {
    if (!user?.token || !newName.trim()) return;
    setSaving(true);

    try {
      const created = await createIngredient(user.token, {
        name: newName,
        category: newCategory || null,
        defaultUnitTypeId: newUnitTypeId || null,
        isNonConvertible: newIsNonConvertible,
        allergenIds: [],
      });

      setSelectedIngredient(created);
      setIngredientSearch(created.name);
      setUnitTypeId(created.defaultUnitTypeId ?? "");
      setShowNewIngredientForm(false);
      setNewName("");
      setNewCategory("");
      setNewUnitTypeId("");
      setNewIsNonConvertible(false);
      toast.info(
        `${created.name} created. Remember to tag allergens in ingredient management.`,
      );
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.errors?.[0]) {
        toast.error(err.response.data.errors[0]);
      } else {
        toast.error("Failed to create ingredient.");
      }
    } finally {
      setSaving(false);
    }
  }

  if (showNewIngredientForm) {
    return (
      <div className="bg-card rounded-lg p-4 border border-border space-y-3">
        <p className="text-sm font-medium text-foreground">
          Create new ingredient
        </p>
        <input
          type="text"
          value={newName}
          onChange={(e) => setNewName(e.target.value)}
          placeholder="Ingredient name"
          className="2-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
        />
        <input
          type="text"
          value={newCategory}
          onChange={(e) => setNewCategory(e.target.value)}
          placeholder="Category (optional)"
          className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
        />
        <select
          value={newUnitTypeId}
          onChange={(e) => setNewUnitTypeId(e.target.value)}
          className="2-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
        >
          <option value="">No default unit</option>
          {unitTypes.map((ut) => (
            <option key={ut.unitTypeId} value={ut.unitTypeId}>
              {ut.name} ({ut.abbreviation})
            </option>
          ))}
        </select>
        <label className="flex items-center gap-2 text-sm text-foreground">
          <input
            type="checkbox"
            checked={newIsNonConvertible}
            onChange={(e) => setNewIsNonConvertible(e.target.checked)}
          />
          Non-convertible unit
        </label>
        <div className="flex gap-2">
          <Button
            onClick={handleCreateIngredient}
            disabled={saving || !newName.trim()}
            className="bg-primary text-primary-foreground"
          >
            Create ingredient
          </Button>
          <Button
            variant="outline"
            onClick={() => setShowNewIngredientForm(false)}
          >
            Cancel
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="space-y-3">
      <div className="relative">
        <input
          type="text"
          value={ingredientSearch}
          onChange={(e) => {
            setIngredientSearch(e.target.value);
            setSelectedIngredient(null);
          }}
          placeholder="Search ingredients..."
          className="w-full px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
        />
        {showDropdown && (
          <div className="absolute z-10 w-full mt-1 bg-card border border-border rounded-lg shadow-lg max-h-48 overflow-y-auto">
            {ingredientResults.length > 0 ? (
              <>
                {ingredientResults.map((ing) => (
                  <button
                    key={ing.ingredientId}
                    onClick={() => handleSelectedIngredient(ing)}
                    className="w-full text-left px-4 py-2 text-sm text-foreground hvoer:bg-muted"
                  >
                    {ing.name}
                    {ing.defaultUnitTypeName && (
                      <span className="text-muted-foreground ml-2">
                        ({ing.defaultUnitTypeName})
                      </span>
                    )}
                  </button>
                ))}
                <button
                  onClick={() => {
                    setNewName(ingredientSearch);
                    setShowNewIngredientForm(true);
                    setShowDropdown(false);
                  }}
                  className="w-full text-left px-4 py-2 text-sm text-secondary hover:bg-muted border-t border-border"
                >
                  + Create "{ingredientSearch}" as new ingredient.
                </button>
              </>
            ) : (
              <button
                onClick={() => {
                  setNewName(ingredientSearch);
                  setShowNewIngredientForm(true);
                  setShowDropdown(false);
                }}
                className="w-full text-left px-4 py-2 text-sm text-secondary hover:bg-muted"
              >
                + Create "{ingredientSearch}" as new ingredient
              </button>
            )}
          </div>
        )}
      </div>

      {selectedIngredient && (
        <div className="flex gap-2">
          <input
            type="number"
            value={quantity}
            onChange={(e) => setQuantity(e.target.value)}
            placeholder="Quantity"
            className="w-28 px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          />
          <select
            value={unitTypeId}
            onChange={(e) => setUnitTypeId(e.target.value)}
            className="flex-1 px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            <option value="">No unit</option>
            {unitTypes.map((ut) => (
              <option key={ut.unitTypeId} value={ut.unitTypeId}>
                {ut.name} ({ut.abbreviation})
              </option>
            ))}
          </select>
          <Button
            onClick={handleAdd}
            disabled={!quantity}
            className="bg-primary text-primary-foreground flex-shrink-0"
          >
            Add
          </Button>
        </div>
      )}
    </div>
  );
}
