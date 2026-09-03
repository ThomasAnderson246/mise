import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { searchIngredients } from "@/api/ingredientApi";
import { Button } from "../ui/button";
import { NewIngredientForm } from "./NewIngredientForm";
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
  //const [newName, setNewName] = useState("");

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
    console.log("handleAdd called", selectedIngredient, quantity);

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
      isRatioAnchor: false,
      isNonConvertible: false,
    };

    onIngredientAdded(newIngredient);
    setSelectedIngredient(null);
    setIngredientSearch("");
    setQuantity("");
    setUnitTypeId("");
  }

  {
    showNewIngredientForm && (
      <NewIngredientForm
        unitTypes={unitTypes}
        initialName={ingredientSearch}
        onCreated={(created) => {
          setSelectedIngredient(created);
          setIngredientSearch(created.name);
          setUnitTypeId(created.defaultUnitTypeId ?? "");
          setShowNewIngredientForm(false);
        }}
        onCancel={() => setShowNewIngredientForm(false)}
      />
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
                    onClick={() => {
                      handleSelectedIngredient(ing);
                      console.log("Click");
                    }}
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
                    console.log("Click");
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
                  console.log("Click!");
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

      {showNewIngredientForm && (
        <NewIngredientForm
          unitTypes={unitTypes}
          initialName={ingredientSearch}
          onCreated={(created) => {
            setSelectedIngredient(created);
            setIngredientSearch(created.name);
            setUnitTypeId(created.defaultUnitTypeId ?? "");
            setShowNewIngredientForm(false);
          }}
          onCancel={() => setShowNewIngredientForm(false)}
        />
      )}

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
