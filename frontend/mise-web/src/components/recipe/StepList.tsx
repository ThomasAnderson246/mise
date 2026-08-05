import type { RecipeStep } from "@/api/recipeApi";

interface StepListProps {
  steps: RecipeStep[];
  onRemove: (stepId: string) => void;
}

export function StepList({ steps, onRemove }: StepListProps) {
  if (steps.length === 0) return null;

  return (
    <ol className="space-y-3 mb-4">
      {steps.map((step, index) => (
        <li
          key={step.stepId}
          className="flex gap-3 bg-card rounded-lg px-4 py-3 border border-border"
        >
          <span className="w-6 h-6 rounded-full bg-primary text-primary-foreground text-xs flex items-center justify-center flex-shrink-0 mt-0.5">
            {index + 1}
          </span>
          <div className="flex-1">
            <p className="text-sm text-foreground">{step.instruction}</p>
            {step.hasTimer && step.timerDuration && (
              <p className="text-xs text-muted-foreground mt-1">
                {step.timerDuration} min
              </p>
            )}
          </div>
          <button
            onClick={() => onRemove(step.stepId)}
            className="text-xs text-destructive hover:underline flex-shrink-0"
          >
            Remove
          </button>
        </li>
      ))}
    </ol>
  );
}
