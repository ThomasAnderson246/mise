import { useTimers } from "@/context/TimerContext";

export function TimerBanner() {
  const { timers, dismissTimer } = useTimers();

  if (timers.length === 0) return null;

  return (
    <div className="fixed top-0 left-0 right-0 z-50 space-y-1">
      {timers.map((timer) => {
        const minutes = Math.floor(timer.remainingSeconds / 60);
        const seconds = timer.remainingSeconds % 60;
        const progress =
          ((timer.totalSeconds - timer.remainingSeconds) / timer.totalSeconds) *
          100;

        return (
          <div
            key={timer.timerId}
            className={`flex items-center gap-3 px-4 py-2 text-sm ${
              timer.isComplete
                ? "bg-green-600 text-white"
                : "bg-primary text-primary-foreground"
            }`}
          >
            <div
              className="absolute bottom-0 left-0 h-0.5 bg-secondary transition-all"
              style={{ width: `${progress}%` }}
            />

            <span className="flex-shrink-0">Timer: </span>
            <div className="flex-1 min-w-0">
              <span className="font-medium">{timer.recipeTitle}</span>
              <span className="text-xs opacity-75 ml-2 truncate">
                {timer.stepInstruction}
              </span>
            </div>
            <span className="font-mono font-medium flex-shrink-0">
              {timer.isComplete
                ? "Done!"
                : `${minutes}:${seconds.toString().padStart(2, "0")}`}
            </span>
            <button
              onClick={() => dismissTimer(timer.timerId)}
              className="text-xs opacity-75 hover:opacity-100 flex-shrink-0"
            >
              X
            </button>
          </div>
        );
      })}
    </div>
  );
}
