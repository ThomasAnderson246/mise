import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { useTimers } from "@/context/TimerContext";
import { useScaling } from "@/hooks/useScaling";
import { getRecipeById } from "@/api/recipeApi";
import { RecipeTimer } from "@/components/RecipeTimer";
import { ScalingControl } from "@/components/recipe/ScalingControl";
import { TimerBanner } from "@/components/TimerBanner";
import type { RecipeDetail } from "@/api/recipeApi";

export default function CookingModePage() {
  const { user } = useAuth();
  const { slug, recipeId } = useParams<{ slug: string; recipeId: string }>();
  const navigate = useNavigate();
  const { timers } = useTimers();

  // state variables
  const [recipe, setRecipe] = useState<RecipeDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [currentStepIndex, setCurrentStepIndex] = useState(0);
  const [showIngredients, setShowIngredients] = useState(true);
  const [showAsyncwarning, setShowAsyncWarning] = useState(false);

  const version = recipe?.currentVersion ?? null;
  const scaling = useScaling(version, recipe?.scalingMode ?? "multiplier");

  const steps = version?.steps ?? [];
  const currentStep = steps[currentStepIndex] ?? null;
  const isFirstStep = currentStepIndex === 0;
  const isLastStep = currentStepIndex === steps.length - 1;

  useEffect(() => {
    if (!user?.token || !recipeId) return;
    getRecipeById(user.token, recipeId)
      .then(setRecipe)
      .catch(() => navigate(-1))
      .finally(() => setLoading(false));
  }, [user, recipeId]);

  function handleNext() {
    if (!currentStep) return;
    console.log("Step:", currentStep.instruction);
    console.log("isAsync:", currentStep.isAsync);
    console.log("hasTimer:", currentStep.hasTimer);
    console.log(
      "Running timers:",
      timers.map((t) => t.stepId),
    );

    //first, check if there's a running async timer
    if (currentStep.isAsync && currentStep.hasTimer) {
      const hasRunningTimer = timers.some(
        (t) => t.stepId === currentStep.stepId,
      );
      if (!hasRunningTimer) {
        setShowAsyncWarning(true);
        return;
      }
    }

    if (!isLastStep) {
      setCurrentStepIndex((prev) => prev + 1);
      setShowAsyncWarning(false);
    }
  }

  function handlePrevious() {
    if (!isFirstStep) {
      setCurrentStepIndex((prev) => prev - 1);
      setShowAsyncWarning(false);
    }
  }

  function handleForceNext() {
    setShowAsyncWarning(false);
    if (!isLastStep) setCurrentStepIndex((prev) => prev + 1);
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-background">
        <p className="text-muted-foreground">Loading...</p>
      </div>
    );
  }

  if (!recipe || !currentStep) {
    return (
      <div className="flex items-center justify-center min-h-screen bg-background">
        <p className="text-muted-foreground">Recipe not found.</p>
      </div>
    );
  }

  const allIngredients = [
    ...(version?.ingredients ?? []),
    ...(version?.recipeIngredientGroups?.flatMap((g) => g.ingredients) ?? []),
  ];

  return (
    <div className="min-h-screen bg-background flex flex-col">
      <TimerBanner />

      <div className="flex items-center justify-between px-6 py-4 border-b border-border">
        <div>
          <h1 className="text-lg font-semibold text-foreground">
            {recipe.title}
          </h1>
          <p className="text-sm text-muted-foreground">
            Step {currentStepIndex + 1} of {steps.length}
          </p>
        </div>
        <div className="flex items-center gap-3">
          <button
            onClick={() => setShowIngredients((prev) => !prev)}
            className="text-sm text-secondary hover:underline"
          >
            {showIngredients ? "Hide ingredients" : "Show ingredients"}
          </button>
          <button
            onClick={() => navigate(`/${slug}/recipes/${recipeId}`)}
            className="text-sm text-muted-foreground hover:text-foreground transition-colors px-3 py-1.5 rounded-lg border border-border"
          >
            Exit
          </button>
        </div>
      </div>

      <div className="flex flex-1 overflow-hidden">
        <div
          className={`flex flex-col flex-1 p-6 md:p-12 ${showIngredients ? "md:w-2/3" : "w-full"} `}
        >
          {currentStep.isAsync && (
            <div className="mb-4 px-4 py-2 rounded-lg bg-yellow-50 border border-secondary text-secondary text-sm">
              This step can run while previous timers are active.
            </div>
          )}

          <div className="flex-1 flex items-center">
            <p className="text-2xl md:text-4xl font-medium text-foreground leading-relaxed">
              {currentStep.instruction}
            </p>
          </div>

          {currentStep.hasTimer && currentStep.timerDuration && (
            <div className="mt-8 max-w-sm">
              <RecipeTimer
                durationMinutes={currentStep.timerDuration}
                stepId={currentStep.stepId}
                recipeTitle={recipe.title}
                instruction={currentStep.instruction}
              />
            </div>
          )}
          {showAsyncwarning && (
            <div className="mt-4 p-4 rounded-lg bg-yellow-50 border border-secondary">
              <p className="text-sm text-secondary mb-3">
                You haven't started the timer for this step. Continue anyway?
              </p>
              <div className="flex gap-2">
                <button
                  onClick={handleForceNext}
                  className="text-sm px-4 py-2 rounded-lg bg-primary text-primary-foreground"
                >
                  Continue without timer
                </button>
                <button
                  onClick={() => setShowAsyncWarning(false)}
                  className="text-sm px-4 py-2 rounded-lg border border-border text-foreground"
                >
                  Go back
                </button>
              </div>
            </div>
          )}

          <div className="flex items-center justify-between mt-8 pt-6 border-t border-border">
            <button
              onClick={handlePrevious}
              disabled={isFirstStep}
              className="flex items-center gap-2 px-6 py-4 rounded-xl border border-border text-foreground disabled:opacity-30 disabled:cursor-not-allowed hover:border-primary transition-colors text-lg font-medium min-w[120px] justify-center"
            >
              Previous
            </button>

            <div className="flex gap-2">
              {steps.map((_, index) => (
                <button
                  key={index}
                  onClick={() => setCurrentStepIndex(index)}
                  className={`w-3 h-3 rounded-full transition-colors ${
                    index === currentStepIndex
                      ? "bg-primary"
                      : index < currentStepIndex
                        ? "bg-secondary"
                        : "bg-muted"
                  }`}
                ></button>
              ))}
            </div>

            {isLastStep ? (
              <button
                onClick={() => navigate(`/${slug}/recipes/${recipeId}`)}
                className="flex items-center gap-2 px-6 py-4 rounded-xl bg-secondary text-secondary-foreground text-lg font-medium min-w-[120px] justify-center"
              >
                Done
              </button>
            ) : (
              <button
                onClick={handleNext}
                className="flex items-center gap-2 px-6 py-4 rounded-xl bg-secondary text-secondary-foreground text-lg font-medium min-w-[120px] justify-center"
              >
                Next
              </button>
            )}
          </div>
        </div>

        {showIngredients && (
          <div className="hidden md:flex flex-col w-1/3 border-1 border-border p-6 overflow-y-auto">
            <h2 className="text-lg font-semibold text-foreground mb-4">
              Ingredients
            </h2>
            "
            <ScalingControl
              isRatioMode={scaling.isRatioMode}
              scalingFactor={scaling.scalingFactor}
              onScalingFactorChange={scaling.setScalingFactor}
              anchorIngredient={scaling.anchorIngredient}
              anchorQuantity={scaling.anchorQuantity}
              onAnchorQuantityChange={scaling.setAnchorQuantity}
            />
            <ul className="space-y-3">
              {allIngredients.map((ing) => (
                <li
                  key={ing.recipeIngredientId}
                  className="flex items-center gap-3 text-sm"
                >
                  <span className="w-20 text-right font-medium text-foreround flex-shrink-0">
                    {scaling.formatQuantity(scaling.getScaledQuantity(ing))}{" "}
                    {ing.unitName ?? ""}
                  </span>
                  <span className="text-foreground">{ing.ingredientName}</span>
                  {ing.isRatioAnchor && scaling.isRatioMode && (
                    <span className="text-xs text-secondary">anchor</span>
                  )}
                </li>
              ))}
            </ul>
          </div>
        )}
      </div>
    </div>
  );
}
