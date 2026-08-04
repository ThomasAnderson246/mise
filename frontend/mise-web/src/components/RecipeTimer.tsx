import { useState, useEffect, useRef } from "react";

interface RecipeTimerProps {
  durationMinutes: number;
}

export function RecipeTimer({ durationMinutes }: RecipeTimerProps) {
  const totalSeconds = durationMinutes * 60;
  const [secondsLeft, setSecondsLeft] = useState(totalSeconds);
  const [isRunning, setIsRunning] = useState(false);
  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  useEffect(() => {
    if (isRunning && secondsLeft > 0) {
      intervalRef.current = setInterval(() => {
        setSecondsLeft((prev) => prev - 1);
      }, 1000);
    } else if (secondsLeft === 0) {
      setIsRunning(false);
    }

    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current);
    };
  }, [isRunning, secondsLeft]);

  function handleStartPause() {
    setIsRunning((prev) => !prev);
  }

  function handleReset() {
    setIsRunning(false);
    setSecondsLeft(totalSeconds);
  }

  const minutes = Math.floor(secondsLeft / 60);
  const seconds = secondsLeft % 60;
  const progress = ((totalSeconds - secondsLeft) / totalSeconds) * 100;
  const isComplete = secondsLeft === 0;

  return (
    <div
      className={`mt-2 flex-center gap-3 p-3 rounded-lg border ${
        isComplete ? "bg-green-50 border-green-200" : "bg-card border-border"
      }`}
    >
      <div className="relative w-10 h-10 flex-shrink-0">
        <svg className="w-10 h-10 -rotate-90" viewBox="0 0 36 36">
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
          {isComplete ? "checkMark" : `${minutes}m`}
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
        {!isComplete && (
          <button
            onClick={handleStartPause}
            className="text-xs px-3 py-1.5 rounded-lg bg-primary text-primary-foreground hover:bg-primary/90 transition-colors"
          >
            {isRunning ? "Pause" : "Start"}
          </button>
        )}
        <button
          onClick={handleReset}
          className="text-xs px-3 py-1.5 rounded-lg bg-primary text-primary-foreground hover:bg-primary/90 transition-colors"
        >
          Reset
        </button>
      </div>
    </div>
  );
}
