import axios from "axios";
import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import {
  getRecipeById,
  createRecipe,
  updateRecipe,
  addIngredient,
  removeIngredient,
  addStep,
  removeStep,
} from "@/api/recipeApi";
import { searchIngredients, createIngredient } from "@/api/ingredientApi";
import { getCategories } from "@/api/categoryApi";
import { getUnitTypes } from "@/api/unitTypeApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import type { RecipeDetail } from "@/api/recipeApi";
import type { IngredientItem } from "@/api/ingredientApi";
import type { CategoryItem } from "@/api/categoryApi";
import type { UnitTypeItem } from "@/api/unitTypeApi";

export default function RecipeEditorPage() {
  const { user } = useAuth();
  const { slug, recipeId } = useParams<{ slug: string; recipeId?: string }>();
  const navigate = useNavigate();
  const isEditMode = !!recipeId;

  // recipe fields
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [scalingMode, setScalingMode] = useState(`multiplier`);
  const [selectedCategories, setSelectedCategories] = useState<string[]>([]);

  //recipe data in edit mode
  const [recipe, setRecipe] = useState<RecipeDetail | null>(null);
  const [currentRecipeId, setCurrentRecipeId] = useState<string | null>(
    recipeId ?? null,
  );

  //reference data
  const [categories, setCategories] = useState<CategoryItem[]>([]);
  const [unitTypes, setUnitTypes] = useState<UnitTypeItem[]>([]);

  //ingredient search
  const [ingredientSearch, setIngredientSearch] = useState("");
  const [ingredientResults, setIngredientResults] = useState<IngredientItem[]>(
    [],
  );
  const [showIngredientDropdown, setShowIngredientDropdown] = useState(false);
  const [selectedIngredient, setSelectedIngredient] =
    useState<IngredientItem | null>(null);
  const [ingredientQuantity, setIngredientQuantity] = useState("");
  const [ingredientUnitTypeId, setIngredientUnitTypeId] = useState("");

  // new ingredient creation
  const [showNewIngredientForm, setShowNewIngredientForm] = useState(false);
  const [newIngredientName, setNewIngredientName] = useState("");
  const [newIngredientCategory, setNewIngredientCategory] = useState("");
  const [newIngredientUnitTypeId, setNewIngredientUnitTypeId] = useState("");
  const [newIngredientIsNonConvertible, setNewIngredientIsNonConvertible] =
    useState(false);
  const [savingMessage, setSavingMessage] = useState<string | null>(null);

  // step form
  const [stepInstruction, setStepInstruction] = useState("");
  const [stepHasTimer, setStepHasTimer] = useState(false);
  const [stepTimerDuration, setStepTimerDuration] = useState("");

  // ui state

  const [loading, setLoading] = useState(isEditMode);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // loading reference data and recipe if in edit mode
  useEffect(() => {
    if (!user?.token) return;

    async function load() {
      try {
        const [catData, unitData] = await Promise.all([
          getCategories(user!.token),
          getUnitTypes(user!.token),
        ]);
        setCategories(catData);
        setUnitTypes(unitData);

        if (isEditMode && recipeId) {
          const recipeData = await getRecipeById(user!.token, recipeId);
          setRecipe(recipeData);
          setTitle(recipeData.title);
          setDescription(recipeData.description ?? "");
          setScalingMode(recipeData.scalingMode);
          setSelectedCategories(
            recipeData.recipeCategories.map((rc) => rc.categoryId),
          );
          console.log("Recipe categories:", recipeData.recipeCategories);
          console.log(
            "Selected categories after load:",
            recipeData.recipeCategories.map((rc) => rc.categoryId),
          );
        }
      } catch {
        setError("Failed to load data.");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [user, recipeId, isEditMode]);

  //ingredient search with debounce
  useEffect(() => {
    if (!user?.token || ingredientSearch.length < 2) {
      setIngredientResults([]);
      setShowIngredientDropdown(false);
      return;
    }

    const timer = setTimeout(async () => {
      try {
        const results = await searchIngredients(user!.token, ingredientSearch);
        setIngredientResults(results);
        setShowIngredientDropdown(true);
      } catch {
        setIngredientResults([]);
      }
    }, 300);

    return () => clearTimeout(timer);
  }, [ingredientSearch, user]);

  async function handleSaveRecipe() {
    if (!user?.token || !title.trim()) return;
    setSaving(true);
    setError(null);

    try {
      if (!currentRecipeId) {
        // creating
        const created = await createRecipe(user.token, {
          title,
          description: description || null,
          scalingMode,
          categoryIds: selectedCategories,
        });
        setCurrentRecipeId(created.recipeId);
        setRecipe(created);
        // redirect to edit mode now, using the new id
        navigate(`/${slug}/recipes/${created.recipeId}/edit`, {
          replace: true,
        });
      } else {
        //edit mode
        console.log("Saving with categories:", selectedCategories);
        await updateRecipe(user.token, currentRecipeId, {
          title,
          description: description || null,
          scalingMode,
          categoryIds: selectedCategories,
        });
        const updated = await getRecipeById(user.token, currentRecipeId);
        setRecipe(updated);
      }
    } catch {
      setError("Failed to save recipe.");
    } finally {
      setSaving(false);
    }
  }

  async function handleAddIngredient() {
    if (
      !user?.token ||
      !currentRecipeId ||
      !selectedIngredient ||
      !ingredientQuantity
    )
      return;
    setSaving(true);

    try {
      await addIngredient(user.token, currentRecipeId, {
        ingredientId: selectedIngredient.ingredientId,
        quantity: parseFloat(ingredientQuantity),
        unitTypeId: ingredientUnitTypeId || null,
        displayOrder: (recipe?.currentVersion?.ingredients.length ?? 0) + 1,
        groupId: null,
        isNonConvertible: selectedIngredient.isNonConvertible,
        isRatioAnchor: false,
      });

      const updated = await getRecipeById(user.token, currentRecipeId);
      setRecipe(updated);

      // reset ingredient form
      setSelectedIngredient(null);
      setIngredientSearch("");
      setIngredientQuantity("");
      setIngredientUnitTypeId("");
    } catch {
      setError("Failed to add ingredient.");
    } finally {
      setSaving(false);
    }
  }

  async function handleCreateAndAddIngredient() {
    if (!user?.token || !newIngredientName.trim()) return;
    setSaving(true);

    try {
      const created = await createIngredient(user.token, {
        name: newIngredientName,
        category: newIngredientCategory || null,
        defaultUnitTypeId: newIngredientUnitTypeId || null,
        isNonConvertible: newIngredientIsNonConvertible,
        allergenIds: [],
      });

      setSelectedIngredient(created);
      setIngredientUnitTypeId(created.defaultUnitTypeId ?? "");
      setIngredientSearch(created.name);
      setShowNewIngredientForm(false);
      setNewIngredientName("");
      setNewIngredientCategory("");
      setNewIngredientUnitTypeId("");
      setNewIngredientIsNonConvertible(false);
      setError(null);
      setSavingMessage(
        `${created.name} created. Remember to tag allergens in ingredient management.`,
      );
      setTimeout(() => setSavingMessage(null), 5000);
      window.scrollTo({ top: 0, behavior: "smooth" });
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.errors?.[0]) {
        setError(err.response.data.errors[0]);
      } else {
        setError("Failed to create ingredient.");
      }
    } finally {
      setSaving(false);
    }
  }

  async function handleRemoveIngredient(recipeIngredientId: string) {
    if (!user?.token || !currentRecipeId) return;

    try {
      await removeIngredient(user.token, currentRecipeId, recipeIngredientId);
      const updated = await getRecipeById(user.token, currentRecipeId);
      setRecipe(updated);
    } catch {
      setError("Failed to remove ingredient.");
    }
  }

  async function handleAddStep() {
    if (!user?.token || !currentRecipeId || !stepInstruction.trim()) return;
    setSaving(true);

    try {
      const stepCount = (recipe?.currentVersion?.steps.length ?? 0) + 1;
      await addStep(user.token, currentRecipeId, {
        stepNumber: stepCount,
        instruction: stepInstruction,
        hasTimer: stepHasTimer,
        timerDuration:
          stepHasTimer && stepTimerDuration
            ? parseInt(stepTimerDuration)
            : null,
        isAsync: false,
        asyncGroupId: null,
      });

      const updated = await getRecipeById(user.token, currentRecipeId);
      setRecipe(updated);

      setStepInstruction("");
      setStepHasTimer(false);
      setStepTimerDuration("");
    } catch {
      setError("Failed to add step.");
    } finally {
      setSaving(false);
    }
  }

  async function handleRemoveStep(stepId: string) {
    if (!user?.token || !currentRecipeId) return;

    try {
      await removeStep(user.token, currentRecipeId, stepId);
      const updated = await getRecipeById(user.token, currentRecipeId);
      setRecipe(updated);
    } catch {
      setError("Failed to remoe step.");
    }
  }

  function toggleCategory(categoryId: string) {
    setSelectedCategories((prev) =>
      prev.includes(categoryId)
        ? prev.filter((id) => id !== categoryId)
        : [...prev, categoryId],
    );
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading...</p>
      </div>
    );
  }

  return (
    <div className="max-w-3xl">
      <PageHeader
        title={isEditMode ? "Edit Recipe" : "New Recipe"}
        action={
          <div className="flex gap-2">
            <Button variant="outline" onClick={() => navigate(-1)}>
              Cancel
            </Button>
            <Button
              onClick={handleSaveRecipe}
              disabled={saving || !title.trim()}
              className="bg-primary text-primary-foreground"
            >
              {saving ? "Saving..." : "Save"}
            </Button>
          </div>
        }
      />

      {error && (
        <div className="mb-4 px-4 py-3 rounded-lg bg-red-50 border border-destructive text-destructive text-sm">
          {error}
        </div>
      )}
      {savingMessage && (
        <div className="mb-4 px-4 py-3 rounded-lg bg-yellow-50 border border-secondary text-secondary text-sm">
          {savingMessage}
        </div>
      )}

      <div className="space-y-4 mb-8">
        <div>
          <label className="block text-sm font-medium text-foreground mb-1">
            Title
          </label>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Recipe title"
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-foreground mb-1">
            Description
          </label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Brief description"
            rows={3}
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring resize-none"
          />
        </div>

        <div>
          <label className="block text-sm font-medium text-foreground mb-1">
            Scaling mode
          </label>
          <select
            value={scalingMode}
            onChange={(e) => setScalingMode(e.target.value)}
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            <option value="multiplier">Multiplier</option>
            <option value="ratio">Ratio</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-foreground mb-2">
            Categories
          </label>
          <div className="flex flex-wrap gap-2">
            {categories.map((cat) => (
              <button
                key={cat.categoryId}
                onClick={() => toggleCategory(cat.categoryId)}
                className={`text-xs px-3 py-1.5 rounded-full border transition-colors ${
                  selectedCategories.includes(cat.categoryId)
                    ? "bg-primary text-primary-foreground border-primary"
                    : "bg-card text-foreground border-border hover:border-primary"
                }`}
              >
                {cat.name}
              </button>
            ))}
          </div>
        </div>
      </div>

      {currentRecipeId && (
        <div className="mb-8">
          <h2 className="text-lg font-semibold text-foreground mb-4 pb-2 border-b border-border">
            Ingredients
          </h2>

          {(recipe?.currentVersion?.ingredients.length ?? 0) > 0 && (
            <ul className="space-y-2 mb-4">
              {recipe?.currentVersion?.ingredients.map((ing) => (
                <li
                  key={ing.recipeIngredientId}
                  className="flex items-center gap-3 bg-card rounded-lg px-4 py-2.5 border border-border"
                >
                  <span className="w-20 text-right text-sm font-medium text-foreground flex-shrink-0">
                    {ing.quantity} {ing.unitName ?? ""}
                  </span>
                  <span className="flex-1 text-sm text-foreground">
                    {ing.ingredientName}
                  </span>
                  <button
                    onClick={() =>
                      handleRemoveIngredient(ing.recipeIngredientId)
                    }
                    className="text-xs text-destructive hover:underline flex-shrink-0"
                  >
                    Remove
                  </button>
                </li>
              ))}
            </ul>
          )}

          {!showNewIngredientForm ? (
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
                {showIngredientDropdown && (
                  <div className="absolute z-10 w-full mt-1 bg-card border border-border rounded-lg shadow-lg max-h-48 overflow-y-auto">
                    {ingredientResults.length > 0 ? (
                      <>
                        {ingredientResults.map((ing) => (
                          <button
                            key={ing.ingredientId}
                            onClick={() => {
                              setSelectedIngredient(ing);
                              setIngredientSearch(ing.name);
                              setIngredientUnitTypeId(
                                ing.defaultUnitTypeId ?? "",
                              );
                              setShowIngredientDropdown(false);
                            }}
                            className="w-full text-left px-4 py-2 text-sm text-foreground hover:bg-muted"
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
                            setNewIngredientName(ingredientSearch);
                            setShowNewIngredientForm(true);
                            setShowIngredientDropdown(false);
                          }}
                          className="w-full text-left px-4 py-2 text-sm text-secondary hover:bg-muted border-t border-border"
                        >
                          + Create "{ingredientSearch}" as new ingredient
                        </button>
                      </>
                    ) : (
                      <button
                        onClick={() => {
                          setNewIngredientName(ingredientSearch);
                          setShowNewIngredientForm(true);
                          setShowIngredientDropdown(false);
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
                    value={ingredientQuantity}
                    onChange={(e) => setIngredientQuantity(e.target.value)}
                    placeholder="Quantity"
                    className="w-28 px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
                  />
                  <select
                    value={ingredientUnitTypeId}
                    onChange={(e) => setIngredientUnitTypeId(e.target.value)}
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
                    onClick={handleAddIngredient}
                    disabled={saving || !ingredientQuantity}
                    className="bg-primary text-primary-foreground flex-shrink-0"
                  >
                    Add
                  </Button>
                </div>
              )}
            </div>
          ) : (
            /* New ingredient form */
            <div className="bg-card rounded-lg p-4 border border-border space-y-3">
              <p className="text-sm font-medium text-foreground">
                Create new ingredient
              </p>
              <input
                type="text"
                value={newIngredientName}
                onChange={(e) => setNewIngredientName(e.target.value)}
                placeholder="Ingredient name"
                className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
              />
              <input
                type="text"
                value={newIngredientCategory}
                onChange={(e) => setNewIngredientCategory(e.target.value)}
                placeholder="Category (optional)"
                className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
              />
              <select
                value={newIngredientUnitTypeId}
                onChange={(e) => setNewIngredientUnitTypeId(e.target.value)}
                className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
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
                  checked={newIngredientIsNonConvertible}
                  onChange={(e) =>
                    setNewIngredientIsNonConvertible(e.target.checked)
                  }
                />
                Non-convertible unit
              </label>
              {error && (
                <div className="px-4 py-3 rounded-lg bg-red-50 border border-destructive text-destructive text-sm">
                  {error}
                </div>
              )}

              <div className="flex gap-2">
                <Button
                  onClick={handleCreateAndAddIngredient}
                  disabled={saving || !newIngredientName.trim()}
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
          )}
        </div>
      )}

      {currentRecipeId && (
        <div className="mb-8">
          <h2 className="text-lg font-semibold text-foreground mb-4 pb-2 border-b border-border">
            Method
          </h2>

          {(recipe?.currentVersion?.steps.length ?? 0) > 0 && (
            <ol className="space-y-3 mb-4">
              {recipe?.currentVersion?.steps.map((step, index) => (
                <li
                  key={step.stepId}
                  className="flex gap-3 bg-card rounded-lg px-4 py-3 border border-border"
                >
                  <span className="w-6 h-6 rounded-full bg-primary text-primary-foreground text-xs flex items-center justify-center flex-shrink-0 mt-0.5">
                    {index + 1}
                  </span>
                  <div className="flex-1">
                    <p className="text-sm text-foreground">
                      {step.instruction}
                    </p>
                    {step.hasTimer && step.timerDuration && (
                      <p className="text-xs text-muted-foreground mt-1">
                        ⏱ {step.timerDuration} min
                      </p>
                    )}
                  </div>
                  <button
                    onClick={() => handleRemoveStep(step.stepId)}
                    className="text-xs text-destructive hover:underline flex-shrink-0"
                  >
                    Remove
                  </button>
                </li>
              ))}
            </ol>
          )}

          <div className="space-y-3">
            <textarea
              value={stepInstruction}
              onChange={(e) => setStepInstruction(e.target.value)}
              placeholder="Step instruction..."
              rows={2}
              className="w-full px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring resize-none"
            />
            <div className="flex items-center gap-4">
              <label className="flex items-center gap-2 text-sm text-foreground">
                <input
                  type="checkbox"
                  checked={stepHasTimer}
                  onChange={(e) => setStepHasTimer(e.target.checked)}
                />
                Has timer
              </label>
              {stepHasTimer && (
                <input
                  type="number"
                  value={stepTimerDuration}
                  onChange={(e) => setStepTimerDuration(e.target.value)}
                  placeholder="Minutes"
                  className="w-28 px-4 py-2 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
                />
              )}
              <Button
                onClick={handleAddStep}
                disabled={saving || !stepInstruction.trim() || !currentRecipeId}
                className="bg-primary text-primary-foreground ml-auto"
              >
                Add step
              </Button>
            </div>
          </div>
        </div>
      )}

      {!currentRecipeId && (
        <div className="bg-card rounded-lg p-6 border border-border text-center">
          <p className="text-sm text-muted-foreground">
            Save the recipe details above to start adding ingredients and steps.
          </p>
        </div>
      )}
    </div>
  );
}
