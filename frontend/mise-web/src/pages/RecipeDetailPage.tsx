import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { getRecipeById, getSubRecipes, publishRecipe } from "@/api/recipeApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import type {
  RecipeDetail,
  SubRecipeItem,
  RecipeVersion,
} from "@/api/recipeApi";
import { toast } from "sonner";
import { RecipeTimer } from "@/components/RecipeTimer";
import { getVersionHistory, restoreVersion } from "@/api/recipeApi";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { getVersionById } from "@/api/recipeApi";
import { useScaling } from "@/hooks/useScaling";
import { ScalingControl } from "@/components/recipe/ScalingControl";

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
  const [selectedVersion, setSelectedVersion] = useState<RecipeVersion | null>(
    null,
  );
  const [loadingVersion, setLoadingVersion] = useState(false);

  // version history
  const [versionHistory, setVersionHistory] = useState<
    {
      versionId: string;
      versionNumber: number;
      isDraft: boolean;
      isPublished: boolean;
      isCurrent: boolean;
      createdAt: string;
      publishedAt: string | null;
      publishedByName: string | null;
    }[]
  >([]);
  const [showVersionHistory, setShowVersionHistory] = useState(false);
  const [restoringVersionId, setRestoringVersionId] = useState<string | null>(
    null,
  );
  const [showRestoreConfirm, setShowRestoreConfirm] = useState<string | null>(
    null,
  );

  useEffect(() => {
    if (!user?.token || !recipeId) return;

    async function loadRecipe() {
      try {
        const [recipeData, subRecipeData, versionData] = await Promise.all([
          getRecipeById(user!.token, recipeId!),
          getSubRecipes(user!.token, recipeId!),
          getVersionHistory(user!.token, recipeId!),
        ]);
        setRecipe(recipeData);
        setSubRecipes(subRecipeData);
        setVersionHistory(versionData);
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

  async function handleRestore(versionId: string) {
    if (!user?.token || !recipeId) return;
    setRestoringVersionId(versionId);

    try {
      await restoreVersion(user.token, recipeId, versionId);
      const updated = await getRecipeById(user.token, recipeId);
      const updatedVersions = await getVersionHistory(user.token, recipeId);
      setRecipe(updated);
      setVersionHistory(updatedVersions);
      setShowRestoreConfirm(null);
      toast.success("Version restored successfully.");
    } catch {
      toast.error("Failed to restore version.");
    } finally {
      setRestoringVersionId(null);
    }
  }

  async function handleViewVersion(versionId: string) {
    if (!user?.token || !recipeId) return;
    setLoadingVersion(true);

    try {
      const data = await getVersionById(user.token, recipeId, versionId);
      setSelectedVersion(data.currentVersion);
    } catch {
      toast.error("Failed to load version.");
    } finally {
      setLoadingVersion(false);
    }
  }
  const version = recipe?.currentVersion ?? null;
  console.log("Scaling mode:", recipe?.scalingMode);
  console.log(
    "Version ingredients:",
    version?.ingredients.map((i) => ({
      name: i.ingredientName,
      isRatioAnchor: i.isRatioAnchor,
    })),
  );
  const scaling = useScaling(version, recipe?.scalingMode ?? "multiplier");

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

            <ScalingControl
              isRatioMode={scaling.isRatioMode}
              scalingFactor={scaling.scalingFactor}
              onScalingFactorChange={scaling.setScalingFactor}
              anchorIngredient={scaling.anchorIngredient}
              anchorQuantity={scaling.anchorQuantity}
              onAnchorQuantityChange={scaling.setAnchorQuantity}
            />

            {version.recipeIngredientGroups.length === 0 &&
            version.ingredients.length === 0 ? (
              <p className="text-sm text-muted-foreground">
                No ingredients added yet.
              </p>
            ) : (
              <div className="space-y-6">
                {version.recipeIngredientGroups?.map((group) => (
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
                            {scaling.formatQuantity(
                              scaling.getScaledQuantity(ing),
                            )}{" "}
                            {ing.unitName ?? ""}
                          </span>
                          <span className="text-foreground">
                            {ing.ingredientName}
                          </span>
                          {ing.isRatioAnchor && (
                            <span className="text-xs text-secondary">
                              anchor
                            </span>
                          )}
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
                          {scaling.formatQuantity(
                            scaling.getScaledQuantity(ing),
                          )}{" "}
                          {ing.unitName ?? ""}
                        </span>
                        <span className="text-foreground">
                          {ing.ingredientName}
                        </span>
                        {ing.isRatioAnchor && (
                          <span className="text-xs text-secondary">anchor</span>
                        )}
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            )}
          </section>

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
      {versionHistory.length > 1 && (
        <section>
          <button
            onClick={() => setShowVersionHistory((prev) => !prev)}
            className="flex items-center gap-2 text-lg font-semibold text-foreground mb-4 pb-2 border-b border-border w-full text-left"
          >
            Version History
            <span className="text-sm text-muted-foreground font-normal ml-auto">
              {versionHistory.length} versions{" "}
              {showVersionHistory ? "up" : "down"}
            </span>
          </button>
          {showVersionHistory && (
            <div className="space-y-2">
              {versionHistory.map((v) => (
                <div
                  key={v.versionId}
                  className={`flex items-center gap-3 p-3 rounded-lg border ${
                    v.isCurrent
                      ? "bg-card border-primary"
                      : "bg-card border-border"
                  }`}
                  onClick={() => !v.isCurrent && handleViewVersion(v.versionId)}
                >
                  <div className="flex-1">
                    <div className="flex items-center gap-2">
                      <span className="text-sm font-medium text-foreground ">
                        Version {v.versionNumber}
                      </span>
                      {v.isCurrent && (
                        <span className="text-xs px-2 py-0.5 rounded-full bg-green text-green-800 font-medium">
                          current
                        </span>
                      )}
                    </div>
                    <p className="text-xs text-muted-foreground mt-0.5">
                      Published{" "}
                      {v.publishedAt
                        ? new Date(v.publishedAt).toLocaleDateString()
                        : new Date(v.createdAt).toLocaleTimeString()}
                      {v.publishedByName && ` by ${v.publishedByName}`}
                    </p>
                  </div>

                  {!v.isCurrent && hasPermission("recipe", "publish") && (
                    <>
                      {showRestoreConfirm === v.versionId ? (
                        <div className="flex items-center gap-2">
                          <span className="text-xs text-muted-foreground">
                            This will discard any active draft.
                          </span>

                          <Button
                            onClick={() => handleRestore(v.versionId)}
                            disabled={restoringVersionId === v.versionId}
                            className="bg-primary text-primary-foreground text-xs h-7 px-3"
                          >
                            {restoringVersionId === v.versionId
                              ? "Restoring..."
                              : "Confirm"}
                          </Button>
                          <Button
                            variant="outline"
                            onClick={() => setShowRestoreConfirm(null)}
                            className="text-xs h-7 px-3"
                          >
                            Cancel
                          </Button>
                        </div>
                      ) : (
                        <Button
                          variant="outline"
                          onClick={() => setShowRestoreConfirm(v.versionId)}
                          className="text-xs h-7 px-3"
                        >
                          Restore
                        </Button>
                      )}
                    </>
                  )}
                </div>
              ))}
            </div>
          )}
        </section>
      )}

      <Dialog
        open={!!selectedVersion}
        onOpenChange={() => setSelectedVersion(null)}
      >
        <DialogContent className="max-w-2xl max-h-[80vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>
              Version{" "}
              {versionHistory.find(
                (v) => v.versionId === selectedVersion?.versionId,
              )?.versionNumber ?? ""}{" "}
              - {recipe.title}{" "}
            </DialogTitle>
          </DialogHeader>

          {loadingVersion ? (
            <p className="text-muted-foreground text-sm">Loading...</p>
          ) : (
            selectedVersion && (
              <div className="space-y-6 mt-4">
                <section>
                  <h3 className="text-sm font-semibold text-foreground mb-3 pb-2 border-b border-border">
                    Ingredients
                  </h3>
                  {selectedVersion.ingredients.length === 0 ? (
                    <p className="text-sm text-muted-foreground">
                      No ingredients.
                    </p>
                  ) : (
                    <ul className="space-y-2">
                      {selectedVersion.ingredients.map((ing) => (
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
                </section>

                <section>
                  <h3 className="text-sm font-semibold text-foreground mb-3 pb-2 border-b border-border">
                    Method
                  </h3>
                  {selectedVersion.steps.length === 0 ? (
                    <p className="text-sm text-muted-foreground">No steps.</p>
                  ) : (
                    <ol className="space-y-3">
                      {selectedVersion.steps.map((step) => (
                        <li key={step.stepId} className="flex gap-3">
                          <span className="w-6 h-6 rounded-full bg-primary text-primary-foreground text-xs flex items-center justify-center flex-shrink-0 mt-0.5">
                            {step.stepNumber}
                          </span>
                          <div className="flex-1">
                            <p className="text-sm text-foreground">
                              {step.instruction}
                            </p>
                            {step.hasTimer && step.timerDuration && (
                              <p className="text-xs text-muted-foreground mt-1">
                                Timer: {step.timerDuration} mins
                              </p>
                            )}
                          </div>
                        </li>
                      ))}
                    </ol>
                  )}
                </section>
              </div>
            )
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}
