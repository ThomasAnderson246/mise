import { useState } from "react";
import { Button } from "../ui/button";
import type { RecipeStep } from "@/api/recipeApi";

interface StepListProps {
  steps: RecipeStep[];
  onRemove: (stepId: string) => void;
  onUpdate: (
    stepId: string,
    instruction: string,
    hasTimer: boolean,
    timerDuration: number | null,
  ) => void;
}

export function StepList({ steps, onRemove, onUpdate }: StepListProps) {
  const [editingId, setEditingId] = useState<string | null>(null);
  const [editInstruction, setEditInstruction] = useState("");
  const [editHasTimer, setEditHasTimer] = useState(false);
  const [editTimerDuration, setEditTimerDuration] = useState("");

  function startEdit(step: RecipeStep) {
    setEditingId(step.stepId);
    setEditInstruction(step.instruction);
    setEditHasTimer(step.hasTimer);
    setEditTimerDuration(step.timerDuration?.toString() ?? "");
  }

  function handleSave(stepId: string) {
    if (!editInstruction.trim()) return;
    onUpdate(
      stepId,
      editInstruction,
      editHasTimer,
      editHasTimer && editTimerDuration ? parseInt(editTimerDuration) : null,
    );
    setEditingId(null);
  }

  function handleCancel() {
    setEditingId(null);
    setEditInstruction("");
    setEditHasTimer(false);
    setEditTimerDuration("");
  }

  if (steps.length === 0) return null;

  return (
    <ol className="space-y-3 mb-4">
      {steps.map((step, index) => (
        <li
          key={step.stepId}
          className="bg-card rounded-lg border border-border overflow-hidden"
        >
          {editingId === step.stepId ? (
            // edit mode
            <div className="p-3 space-y-2">
              <div className="flex gap-2 items-start">
                <span className="w-6 h-6 rounded-full bg-primary text-primary-foreground text-xs flex items-center justify-center flex-shrink-0 mt-1">
                  {index + 1}
                </span>
                <textarea
                  value={editInstruction}
                  onChange={(e) => setEditInstruction(e.target.value)}
                  rows={2}
                  className="flex-1 px-3 py-2 rounded border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring resize-none"
                  autoFocus
                />
              </div>
              <div className="flex items-center gap-4 pl-8">
                <label className="flex items-center gap-2 text-sm text-foreground">
                  <input
                    type="checkbox"
                    checked={editHasTimer}
                    onChange={(e) => setEditHasTimer(e.target.checked)}
                  />
                  Has Timer
                </label>
                {editHasTimer && (
                  <input
                    type="number"
                    value={editTimerDuration}
                    onChange={(e) => setEditTimerDuration(e.target.value)}
                    placeholder="Minutes"
                    className="w-24 px-3 py-1 rounded border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
                  />
                )}
                <div className="flex gap-2 ml-auto">
                  <Button
                    onClick={() => handleSave(step.stepId)}
                    disabled={!editInstruction.trim()}
                    className="bg-primary text-primary-foreground text-xs h-8 px-3"
                  >
                    Save
                  </Button>
                  <button
                    onClick={handleCancel}
                    className="text-xs text-muted-foreground hover:text-foreground"
                  >
                    Cancel
                  </button>
                </div>
              </div>
            </div>
          ) : (
            // display mode
            <div className="flex gap-3 px-4 py-3">
              <span className="w-6 h-6 rounded-full bg-primary text-primary-foreground text-xs flex items-center justify-center flex-shrink-0 mt-0.5">
                {index + 1}
              </span>
              <div className="flex-1">
                <p className="text-sm text-foreground">{step.instruction}</p>
                {step.hasTimer && step.timerDuration && (
                  <p className="text-xs text-muted-foregound mt-1">
                    {step.timerDuration} min
                  </p>
                )}
              </div>
              <button
                onClick={() => startEdit(step)}
                className="text-xs text-secondary hover:underline flex-shrink-0"
              >
                Edit
              </button>
              <button
                onClick={() => onRemove(step.stepId)}
                className="text-xs text-destructive hover:underline flex-shrink-0"
              >
                Remove
              </button>
            </div>
          )}
        </li>
      ))}
    </ol>
  );
}
