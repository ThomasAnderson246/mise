import { useState } from "react";
import { Button } from "../ui/button";
import type { RecipeStep } from "@/api/recipeApi";

interface StepFormProps {
  onStepAdded: (step: RecipeStep) => void;
  currentStepCount: number;
}

export function StepForm({ onStepAdded, currentStepCount }: StepFormProps) {
  const [instruction, setInstruction] = useState("");
  const [hasTimer, setHasTimer] = useState(false);
  const [timerDuration, setTimerDuration] = useState("");

  function handleAdd() {
    if (!instruction.trim()) return;

    const newStep: RecipeStep = {
      stepId: crypto.randomUUID(),
      stepNumber: currentStepCount + 1,
      instruction,
      hasTimer,
      timerDuration: hasTimer && timerDuration ? parseInt(timerDuration) : null,
      isAsync: false,
      asyncGroupId: null,
    };

    onStepAdded(newStep);
    setInstruction("");
    setHasTimer(false);
    setTimerDuration("");
  }

  return (
    <div className="space-y-3">
      <textarea
        value={instruction}
        onChange={(e) => setInstruction(e.target.value)}
        placeholder="Step instruction..."
        rows={2}
        className="w-full px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring resize-none"
      />
      <div className="flex items-center gap-4">
        <label className="flex items-center gap-2 text-sm text-foreground">
          <input
            type="checkbox"
            checked={hasTimer}
            onChange={(e) => setHasTimer(e.target.checked)}
          />
          Has Timer
        </label>
        {hasTimer && (
          <input
            type="number"
            value={timerDuration}
            onChange={(e) => setTimerDuration(e.target.value)}
            placeholder="Minutes"
            className="w-28 px-4 py-2 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          />
        )}
        <Button
          onClick={handleAdd}
          disabled={!instruction.trim()}
          className="bg-primary text-primary-foreground ml-auto"
        >
          Add step
        </Button>
      </div>
    </div>
  );
}
