import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { NewIngredientForm } from "../recipe/NewIngredientForm";
import { Button } from "../ui/button";
import { getUnitTypes } from "@/api/unitTypeApi";
import {
  getIngredients,
  addAllergenToIngredient,
  removeAllergenFromIngredient,
} from "@/api/ingredientApi";
import { getAllAllergens } from "@/api/allergenApi";
import { toast } from "sonner";
import type { IngredientItem } from "@/api/ingredientApi";
import type { AllergenTagItem } from "@/api/allergenApi";
import type { UnitTypeItem } from "@/api/unitTypeApi";

export function IngredientsTab() {
  const { user } = useAuth();
  const [ingredients, setIngredients] = useState<IngredientItem[]>([]);
  const [allergens, setAllergens] = useState<AllergenTagItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [expandedId, setExpandedId] = useState<string | null>(null);
  const [unitTypes, setUnitTypes] = useState<UnitTypeItem[]>([]);
  const [showCreateForm, setShowCreateForm] = useState(false);

  useEffect(() => {
    if (!user?.token) return;

    async function load() {
      try {
        const [ingData, allergenData, unitData] = await Promise.all([
          getIngredients(user!.token),
          getAllAllergens(user!.token),
          getUnitTypes(user!.token),
        ]);
        setIngredients(ingData);
        setAllergens(allergenData);
        setUnitTypes(unitData);
      } catch {
        toast.error("Error loading data.");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [user]);

  async function handleAddAllergen(ingredientId: string, allergenId: string) {
    if (!user?.token) return;
    try {
      const updated = await addAllergenToIngredient(
        user.token,
        ingredientId,
        allergenId,
      );
      setIngredients((prev) =>
        prev.map((i) => (i.ingredientId === ingredientId ? updated : i)),
      );
      toast.success("Allergen tagged.");
    } catch {
      toast.error("Failed to tag allergen.");
    }
  }

  async function handleRemoveAllergen(
    ingredientId: string,
    allergenId: string,
  ) {
    if (!user?.token) return;
    try {
      const updated = await removeAllergenFromIngredient(
        user.token,
        ingredientId,
        allergenId,
      );
      setIngredients((prev) =>
        prev.map((i) => (i.ingredientId === ingredientId ? updated : i)),
      );
      toast.success("Allergen removed.");
    } catch {
      toast.error("Failed to remove allergen.");
    }
  }

  const filtered = ingredients.filter((i) =>
    i.name.toLowerCase().includes(search.toLowerCase()),
  );

  if (loading)
    return <p className="text-sm text-muted-foreground">Loading...</p>;

  return (
    <div className="max-w-lg space-y-4">
      {showCreateForm ? (
        <NewIngredientForm
          unitTypes={unitTypes}
          onCreated={(created) => {
            setIngredients((prev) => [...prev, created]);
            setShowCreateForm(false);
          }}
          onCancel={() => setShowCreateForm(false)}
        />
      ) : (
        <Button
          variant="outline"
          onClick={() => setShowCreateForm(true)}
          className="w-full mb-4"
        >
          +Add ingredient
        </Button>
      )}

      <input
        type="text"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        placeholder="Search ingredients..."
        className="w-full px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
      />

      <div className="space-y-2">
        {filtered.length === 0 ? (
          <p className="text-sm text-muted-foreground">No ingredients found.</p>
        ) : (
          filtered.map((ing) => {
            const isExpanded = expandedId === ing.ingredientId;
            const assignedAllergenids = new Set(
              ing.allergens.map((a) => a.allergenId),
            );
            const availableAllergens = allergens.filter(
              (a) => !assignedAllergenids.has(a.allergenId),
            );

            return (
              <div
                key={ing.ingredientId}
                className="bg-card rounded-lg border border-border overflow-hidden"
              >
                <button
                  onClick={() =>
                    setExpandedId(isExpanded ? null : ing.ingredientId)
                  }
                  className="w-full flex items-center justify-between p-3 text-left hover:bg-muted transition-colors"
                >
                  <div className="flex items-center gap-3">
                    <span className="text-sm font-medium text-foreground">
                      {ing.name}
                    </span>
                    {ing.allergens.length > 0 && (
                      <span className="text-xs text-muted-foreground">
                        {ing.allergens.length} allergen
                        {ing.allergens.length !== 1 ? "s" : ""}
                      </span>
                    )}
                    {ing.allergens.length === 0 && (
                      <span className="text-xs text-yellow-600">
                        no allergens tagged
                      </span>
                    )}
                  </div>
                  <span className="text-muted-foreground">
                    {isExpanded ? "up" : "down"}
                  </span>
                </button>

                {isExpanded && (
                  <div className="border-t border-border p-3 space-y-3">
                    {/* Current allergens*/}
                    {ing.allergens.length > 0 && (
                      <div className="flex flex-wrap gap-2">
                        {ing.allergens.map((a) => (
                          <div
                            key={a.allergenId}
                            className="flex items-center gap-1"
                          >
                            <span
                              className={`text-xs px-2 py-0.5 rounded-full ${
                                a.isMajor
                                  ? "bg-red-100 text-red-800"
                                  : "bg-orange-100 text-orange-800"
                              }`}
                            >
                              {a.name}
                            </span>
                            <button
                              onClick={() =>
                                handleRemoveAllergen(
                                  ing.ingredientId,
                                  a.allergenId,
                                )
                              }
                              className="text-xs text-destructive hover:underline"
                            >
                              X
                            </button>
                          </div>
                        ))}
                      </div>
                    )}

                    {/* add allergen*/}
                    {availableAllergens.length > 0 && (
                      <select
                        defaultValue=""
                        onChange={(e) => {
                          if (e.target.value) {
                            handleAddAllergen(ing.ingredientId, e.target.value);
                            e.target.value = "";
                          }
                        }}
                        className="w-full px-3 py-2 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
                      >
                        <option value="">+ Add allergen tag...</option>
                        {availableAllergens.map((a) => (
                          <option key={a.allergenId} value={a.allergenId}>
                            {a.name} {a.isMajor ? "(major)" : ""}
                          </option>
                        ))}
                      </select>
                    )}
                  </div>
                )}
              </div>
            );
          })
        )}
      </div>
    </div>
  );
}
