import { useTimers } from "@/context/TimerContext";

interface RecipeTimerProps {
  durationMinutes: number;
  stepId: string;
  recipeTitle: string;
  instruction: string;
}

export function RecipeTimer({
  durationMinutes,
  stepId,
  recipeTitle,
  instruction,
}: RecipeTimerProps) {
  const { timers, startTimer, dismissTimer, pauseTimer, resumeTimer } =
    useTimers();
  const activeTimer = timers.find((t) => t.stepId === stepId);

  const totalSeconds = durationMinutes * 60;
  const remainingSeconds = activeTimer?.remainingSeconds ?? totalSeconds;
  const isComplete = activeTimer?.isComplete ?? false;
  const isPaused = activeTimer?.isPaused ?? false;
  //const isRunning = !!activeTimer && !isComplete && !isPaused;

  const minutes = Math.floor(remainingSeconds / 60);
  const seconds = remainingSeconds % 60;
  const progress = activeTimer
    ? ((totalSeconds - remainingSeconds) / totalSeconds) * 100
    : 0;

  function handleStart() {
    startTimer(stepId, recipeTitle, instruction, durationMinutes);
  }

  return (
    <div
      className={`mt-2 flex items-center gap-3 p-3 rounded-lg border ${
        isComplete ? "bg-green-50 border-green-200" : "bg-card border-border"
      }`}
    >
      <div className="relative w-10 h-10 flex-shrink-0">
        <svg className="w-10 h-10 -rotat-90" viewBox="0 0 0 36">
          <circle
            cx="18"
            cy="18"
            r="15.9"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            className="text-muted"
          />
          <circle
            cx="18"
            cy="18"
            r="15.9"
            fill="none"
            stroke="currentColor"
            strokeWidth="2"
            strokeDasharray={`${progress} 100`}
            className={isComplete ? "text-green-500" : "text-secondary"}
            strokeLinecap="round"
          />
        </svg>
        <span className="absolute inset-0 flex items-center justify-center text-xs font-medium text-foreground">
          {isComplete ? "yes" : `${minutes}m`}
        </span>
      </div>

      <div className="flex-1">
        <p
          className={`text-sm font-medium ${isComplete ? "text-green-700" : "text-foreground"}`}
        >
          {isComplete
            ? "Done!"
            : `${minutes}:${seconds.toString().padStart(2, "0")}`}
        </p>
        <p className="text-xs text-muted-foreground">
          {durationMinutes} min timer
        </p>
      </div>

      <div className="flex gap-2">
        {!activeTimer && !isComplete && (
          <button
            onClick={handleStart}
            className="text-xs px-3 py-1.5 rounded-lg bg-primary text-primary-foreground hover:bg-primary/90 transition-colors"
          >
            Start
          </button>
        )}
        {activeTimer && !isComplete && (
          <button
            onClick={() =>
              isPaused
                ? resumeTimer(activeTimer.timerId)
                : pauseTimer(activeTimer.timerId)
            }
            className="text-xs px-3 py-1.5 rounded-lg bg-primary text-primary-foreground hover:bg-primary/90 transition-colors"
          >
            {isPaused ? "Resume" : "Pause"}
          </button>
        )}
        {(activeTimer || isComplete) && (
          <button
            onClick={() => activeTimer && dismissTimer(activeTimer.timerId)}
            className="text-xs px-3 py-1.5 rounded-lg bg-primary text-primary-foreground hover:bg-primary/90 transition-colors"
          >
            {isComplete ? "Dismiss" : "X"}
          </button>
        )}
      </div>
    </div>
  );
}
