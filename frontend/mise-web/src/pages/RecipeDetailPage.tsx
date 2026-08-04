import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { getRecipeById, getSubRecipes, publishRecipe } from "@/api/recipeApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import type { RecipeDetail, SubRecipeItem } from "@/api/recipeApi";
import { toast } from "sonner";
import { RecipeTimer } from "@/components/RecipeTimer";

export default function RecipeDetailPage() {
  const { user, hasPermission } = useAuth();
  const { slug, recipeId } = useParams<{ slug: string; recipeId: string }>();
  const navigate = useNavigate();

  const [recipe, setRecipe] = useState<RecipeDetail | null>(null);
  const [subRecipes, setSubRecipes] = useState<SubRecipeItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [publishing, setPublishing] = useState(false);
  const [showPublishConfirm, setShowPublishConfirm] = useState(false);

  useEffect(() => {
    if (!user?.token || !recipeId) return;

    async function loadRecipe() {
      try {
        const [recipeData, subRecipeData] = await Promise.all([
          getRecipeById(user!.token, recipeId!),
          getSubRecipes(user!.token, recipeId!),
        ]);
        setRecipe(recipeData);
        setSubRecipes(subRecipeData);
      } catch {
        setError("Recipe not found.");
      } finally {
        setLoading(false);
      }
    }
    loadRecipe();
  }, [user, recipeId]);

  async function handlePublish() {
    if (!user?.token || !recipeId) return;
    setPublishing(true);

    try {
      await publishRecipe(user.token, recipeId);
      const updated = await getRecipeById(user.token, recipeId);
      setRecipe(updated);
      setShowPublishConfirm(false);
      toast.success("Recipe published successfully.");
    } catch {
      toast.error("Failed to publish recipe.");
    } finally {
      setPublishing(false);
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading recipe...</p>
      </div>
    );
  }

  if (error || !recipe) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">{error ?? "Recipe not found."}</p>
      </div>
    );
  }

  const version = recipe.currentVersion;

  return (
    <div className="max-w-3xl">
      <PageHeader
        title={recipe.title}
        subtitle={recipe.description ?? undefined}
        action={
          <div className="flex gap-2">
            {hasPermission("recipe", "update") && (
              <Button
                variant="outline"
                onClick={() => navigate(`/${slug}/recipes/${recipeId}/edit`)}
              >
                Edit
              </Button>
            )}
            {hasPermission("recipe", "publish") &&
              recipe.status === "draft" && (
                <>
                  {showPublishConfirm ? (
                    <div className="flex items-center gap-2">
                      <span className="text-sm text-muted-foreground">
                        Are you sure?
                      </span>
                      <Button
                        onClick={handlePublish}
                        disabled={publishing}
                        className="bg-primary text-primary-foreground"
                      >
                        {publishing ? "Publishing..." : "Confirm"}
                      </Button>
                    </div>
                  ) : (
                    <Button
                      onClick={() => setShowPublishConfirm(true)}
                      className="bg-primary text-primary-foreground"
                    >
                      Publish
                    </Button>
                  )}
                </>
              )}
          </div>
        }
      />

      <div className="flex items-center gap-3 mb-8 flex-wrap">
        <span
          className={`text-xs px-2 py-0.5 rounded-full font-medium ${
            recipe.status === "published"
              ? "bg-green-100 text-green-800"
              : "bg-yellow-100 text-yellow-800"
          }`}
        >
          {recipe.status}
        </span>
        {recipe.recipeCategories?.map((rc) => (
          <span
            key={rc.categoryId}
            className="text-xs px-2 py-0.5 rounded-full bg-muted text-muted-foreground"
          >
            {rc.name}
          </span>
        ))}
        <span className="text-xs text-muted-foreground ml-auto">
          Scaling: {recipe.scalingMode}
        </span>
      </div>

      {!version ? (
        <div className="bg-card rounded-lg p-6 border border-border text-center">
          <p className="text-muted-foreground text-sm">
            This recipe has no version yet.
          </p>
        </div>
      ) : (
        <div className="space-y-8">
          <section>
            <h2 className="text-lg font-semibold text-foreground mb-4 pb-2 border-b border-border">
              Ingredients
            </h2>

            {version.recipeIngredientGroups.length === 0 &&
            version.ingredients.length === 0 ? (
              <p className="text-sm text-muted-foreground">
                No ingredients added yet.
              </p>
            ) : (
              <div className="space-y-6">
                {/* Grouped ingredients */}
                {version.recipeIngredientGroups.map((group) => (
                  <div key={group.groupId}>
                    <h3 className="text-sm font-medium text-secondary mb-2 uppercase tracking-wide">
                      {group.name}
                    </h3>
                    <ul className="space-y-2">
                      {group.ingredients.map((ing) => (
                        <li
                          key={ing.recipeIngredientId}
                          className="flex items-center gap-3 text-sm"
                        >
                          <span className="w-24 text-right font-medium text-foreground flex-shrink-0">
                            {ing.quantity} {ing.unitName ?? ""}
                          </span>
                          <span className="text-foreground">
                            {ing.ingredientName}
                          </span>
                        </li>
                      ))}
                    </ul>
                  </div>
                ))}

                {version.ingredients.length > 0 && (
                  <ul className="space-y-2">
                    {version.ingredients.map((ing) => (
                      <li
                        key={ing.recipeIngredientId}
                        className="flex items-center gap-3 text-sm"
                      >
                        <span className="w-24 text-right font-medium text-foreground flex-shrink-0">
                          {ing.quantity} {ing.unitName ?? ""}
                        </span>
                        <span className="text-foreground">
                          {ing.ingredientName}
                        </span>
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            )}
          </section>
          {console.log(
            "Steps:",
            version.steps.map((s) => ({
              stepNumber: s.stepNumber,
              hasTimer: s.hasTimer,
              timerDuration: s.timerDuration,
            })),
          )}
          <section>
            <h2 className="text-lg font-semibold text-foreground mb-4 pb-2 border-b border-border">
              Method
            </h2>
            {version.steps.length === 0 ? (
              <p className="text-sm text-muted-foreground">
                No steps added yet.
              </p>
            ) : (
              <ol className="space-y-4">
                {version.steps.map((step) => (
                  <li key={step.stepId} className="flex gap-4">
                    <span className="w-6 h-6 rounded-full bg-primary text-primary-foreground text-xs flex items-center justify-center flex-shrink-0 mt-0.5">
                      {step.stepNumber}
                    </span>
                    <div className="flex-1">
                      <p className="text-sm text-foreground">
                        {step.instruction}
                      </p>
                      <div className="flex items-center gap-3 mt-1">
                        {step.hasTimer && step.timerDuration && (
                          <RecipeTimer durationMinutes={step.timerDuration} />
                        )}
                        {step.isAsync && (
                          <span className="text-xs text-secondary">
                            ↕ Async
                          </span>
                        )}
                      </div>
                    </div>
                  </li>
                ))}
              </ol>
            )}
          </section>

          {/* Sub-recipes */}
          {subRecipes.length > 0 && (
            <section>
              <h2 className="text-lg font-semibold text-foreground mb-4 pb-2 border-b border-border">
                Sub-recipes
              </h2>
              <ul className="space-y-2">
                {subRecipes.map((sr) => (
                  <li key={sr.subRecipeId}>
                    <button
                      onClick={() =>
                        navigate(`/${slug}/recipes/${sr.subRecipeId}`)
                      }
                      className="text-sm text-secondary hover:underline flex items-center gap-2"
                    >
                      {sr.subRecipeTitle}
                      <span
                        className={`text-xs px-2 py-0.5 rounded-full ${
                          sr.subRecipeStatus === "published"
                            ? "bg-green-100 text-green-800"
                            : "bg-yellow-100 text-yellow-800"
                        }`}
                      >
                        {sr.subRecipeStatus}
                      </span>
                    </button>
                  </li>
                ))}
              </ul>
            </section>
          )}
        </div>
      )}
    </div>
  );
}
