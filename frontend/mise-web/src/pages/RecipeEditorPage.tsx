import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import {
  getRecipeById,
  createRecipe,
  updateRecipe,
  getDraft,
  createDraft,
  saveDraft,
  discardDraft,
  publishRecipe,
} from "@/api/recipeApi";
import { getCategories } from "@/api/categoryApi";
import { getUnitTypes } from "@/api/unitTypeApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import { IngredientList } from "@/components/recipe/IngredientList";
import { IngredientSearch } from "@/components/recipe/IngredientSearch";
import { StepList } from "@/components/recipe/StepList";
import { StepForm } from "@/components/recipe/StepForm";
import { toast } from "sonner";
import { inputClass, selectClass } from "@/lib/styles";
import type { RecipeIngredient, RecipeStep } from "@/api/recipeApi";
import type { CategoryItem } from "@/api/categoryApi";
import type { UnitTypeItem } from "@/api/unitTypeApi";

export default function RecipeEditorPage() {
  const { user } = useAuth();
  const { slug, recipeId } = useParams<{ slug: string; recipeId?: string }>();
  const navigate = useNavigate();
  const isEditMode = !!recipeId;

  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [scalingMode, setScalingMode] = useState("multiplier");
  const [selectedCategories, setSelectedCategories] = useState<string[]>([]);

  const [currentRecipeId, setCurrentRecipeId] = useState<string | null>(
    recipeId ?? null,
  );
  const [draftVersionId, setDraftVersionId] = useState<string | null>(null);
  const [isPublishedRecipe, setIsPublishedRecipe] = useState(false);

  // local state
  const [localIngredients, setLocalIngredients] = useState<RecipeIngredient[]>(
    [],
  );
  const [localSteps, setLocalSteps] = useState<RecipeStep[]>([]);
  const [hasUnsavedChanges, setHasUnsavedChanges] = useState(false);

  //reference data
  const [categories, setCategories] = useState<CategoryItem[]>([]);
  const [unitTypes, setUnitTypes] = useState<UnitTypeItem[]>([]);

  // ui state
  const [loading, setLoading] = useState(isEditMode);
  const [saving, setSaving] = useState(false);
  const [showDiscardConfirm, setShowDiscardConfirm] = useState(false);
  const [publishing, setPublishing] = useState(false);

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

          setTitle(recipeData.title);
          setDescription(recipeData.description ?? "");
          setScalingMode(recipeData.scalingMode);
          setSelectedCategories(
            recipeData.recipeCategories.map((rc) => rc.categoryId),
          );
          setIsPublishedRecipe(recipeData.status === "published");

          if (recipeData.status === "published") {
            const existingDraft = await getDraft(user!.token, recipeId);
            if (existingDraft?.currentVersion) {
              setDraftVersionId(existingDraft.currentVersion.versionId);
              setLocalIngredients(existingDraft.currentVersion.ingredients);
              setLocalSteps(existingDraft.currentVersion.steps);
              setHasUnsavedChanges(true);
              toast.info("Resuming unsaved draft from a previous session.");
            } else {
              await createDraft(user!.token, recipeId);
              const newDraft = await getDraft(user!.token, recipeId);
              if (newDraft?.currentVersion) {
                setDraftVersionId(newDraft.currentVersion.versionId);
                setLocalIngredients(newDraft.currentVersion.ingredients);
                setLocalSteps(newDraft.currentVersion.steps);
              }
            }
          } else {
            setDraftVersionId(recipeData.currentVersion?.versionId ?? null);
            setLocalIngredients(recipeData.currentVersion?.ingredients ?? []);
            setLocalSteps(recipeData.currentVersion?.steps ?? []);
          }
        }
      } catch {
        toast.error("Failed to load recipe data.");
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [user, recipeId, isEditMode]);

  async function handleSaveRecipe() {
    if (!user?.token || !title.trim()) return;
    setSaving(true);
    console.log("Saving with draftVersionId:", draftVersionId);
    console.log("currentRecipeId:", currentRecipeId);

    try {
      if (!currentRecipeId) {
        const created = await createRecipe(user.token, {
          title,
          description: description || null,
          scalingMode,
          categoryIds: selectedCategories,
        });
        setCurrentRecipeId(created.recipeId);

        setDraftVersionId(created.currentVersion?.versionId ?? null);
        setLocalIngredients(created.currentVersion?.ingredients ?? []);
        setLocalSteps(created.currentVersion?.steps ?? []);
        navigate(`/${slug}/recipes/${created.recipeId}/edit`, {
          replace: true,
        });
        toast.success("Recipe created.");
      } else {
        await updateRecipe(user.token, currentRecipeId, {
          title,
          description: description || null,
          scalingMode,
          categoryIds: selectedCategories,
        });

        if (draftVersionId) {
          await saveDraft(user.token, currentRecipeId, draftVersionId, {
            ingredients: localIngredients.map((ing, index) => ({
              recipeIngredientId: ing.recipeIngredientId,
              ingredientId: ing.ingredientId ?? "",
              quantity: ing.quantity,
              unitTypeId: ing.unitTypeId ?? null,
              displayOrder: index + 1,
              groupId: ing.groupId ?? null,
              isNonConvertible: false,
              isRatioAnchor: false,
            })),
            steps: localSteps.map((step, index) => ({
              stepId: step.stepId,
              stepNumber: index + 1,
              instruction: step.instruction,
              hasTimer: step.hasTimer,
              timerDuration: step.timerDuration,
              isAsync: step.isAsync,
              asyncGroupId: step.asyncGroupId ?? null,
            })),
            ingredientGroups: [],
          });
        }

        setHasUnsavedChanges(false);
        toast.success("Recipe saved.");
      }
    } catch {
      toast.error("Failed to save recipe.");
    } finally {
      setSaving(false);
    }
  }

  async function handleDiscardDraft() {
    if (!user?.token || !currentRecipeId) return;

    try {
      if (draftVersionId) {
        await discardDraft(user.token, currentRecipeId);
      }
    } catch {
      // try to remove db version first, if one exists
      // no draft saved to db, move on to local state
    } finally {
      const recipeData = await getRecipeById(user.token, currentRecipeId);
      setLocalIngredients(recipeData.currentVersion?.ingredients ?? []);
      setLocalSteps(recipeData.currentVersion?.steps ?? []);
      setDraftVersionId(null);
      setHasUnsavedChanges(false);
      toast.success("Changes discarded");
    }
  }

  function handleIngredientAdded(ingredient: RecipeIngredient) {
    setLocalIngredients((prev) => [...prev, ingredient]);
    setHasUnsavedChanges(true);
  }

  function handleIngredientRemoved(recipeIngredientId: string) {
    setLocalIngredients((prev) =>
      prev.filter((i) => i.recipeIngredientId !== recipeIngredientId),
    );
    setHasUnsavedChanges(true);
  }

  function handleStepAdded(step: RecipeStep) {
    setLocalSteps((prev) => [...prev, step]);
    setHasUnsavedChanges(true);
  }

  function handleStepRemoved(stepId: string) {
    setLocalSteps((prev) => prev.filter((s) => s.stepId !== stepId));
    setHasUnsavedChanges(true);
  }

  function toggleCategory(categoryId: string) {
    setSelectedCategories((prev) =>
      prev.includes(categoryId)
        ? prev.filter((id) => id !== categoryId)
        : [...prev, categoryId],
    );
  }

  async function handlePublish() {
    if (!user?.token || !currentRecipeId) return;
    setPublishing(true);

    try {
      await handleSaveRecipe();
      await publishRecipe(user.token, currentRecipeId);
      toast.success("Recipe published successfully.");
      navigate(`/${slug}/recipes/${currentRecipeId}`);
    } catch {
      toast.error("Failed to publish recipe.");
    } finally {
      setPublishing(false);
    }
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
            {isPublishedRecipe && (
              <>
                {showDiscardConfirm ? (
                  <div className="flex items-center gap-2">
                    <span className="text-sm text-muted-foreground">
                      Discard all changes?
                    </span>
                    <Button
                      variant="outline"
                      onClick={handleDiscardDraft}
                      className="text-destructive border-destructive"
                    >
                      Discard
                    </Button>
                    <Button
                      variant="outline"
                      onClick={() => setShowDiscardConfirm(false)}
                    >
                      Cancel
                    </Button>
                  </div>
                ) : (
                  hasUnsavedChanges && (
                    <Button
                      variant="outline"
                      onClick={() => setShowDiscardConfirm(true)}
                      className="text-destructive border-destructive"
                    >
                      Discard changes
                    </Button>
                  )
                )}
              </>
            )}
            <Button variant="outline" onClick={() => navigate(-1)}>
              Back
            </Button>
            {(hasUnsavedChanges || !currentRecipeId) && (
              <Button
                onClick={handleSaveRecipe}
                disabled={saving || !title.trim()}
                className="bg-primary text-primary-foreground"
              >
                {saving ? "Saving..." : "Save"}
              </Button>
            )}
            {isPublishedRecipe && draftVersionId && !hasUnsavedChanges && (
              <Button
                onClick={handlePublish}
                disabled={publishing || !title.trim()}
                className="bg-secondary text-secondary-foreground"
              >
                {publishing ? "Publishing..." : "Publish"}
              </Button>
            )}
          </div>
        }
      />

      {hasUnsavedChanges && (
        <div className="mb-4 px-4 py-3 rounded-lg bg-yellow-50 border border-secondary text-sm">
          You have unsaved changes. Click Save to commit them.
        </div>
      )}

      {isPublishedRecipe && draftVersionId && (
        <div className="mb-4 px-4 py-3 rounded-lg bg-card border border-border text-sm text-muted-foreground">
          Editing draft - changes wont' go live until you publish.
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
            className={inputClass}
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
            className={selectClass}
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
                className={`text-xs px-3 py-1.6 rounded-full border transition-colors ${
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
          <IngredientList
            ingredients={localIngredients}
            onRemove={handleIngredientRemoved}
          />
          <IngredientSearch
            unitTypes={unitTypes}
            currentIngredientCount={localIngredients.length}
            onIngredientAdded={handleIngredientAdded}
          />
        </div>
      )}

      {currentRecipeId && (
        <div className="mb-8">
          <h2 className="text-lg font-semibold text-foreground mb-4 pb-2 border-b border-border">
            Method
          </h2>
          <StepList steps={localSteps} onRemove={handleStepRemoved} />
          <StepForm
            onStepAdded={handleStepAdded}
            currentStepCount={localSteps.length}
          />
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
