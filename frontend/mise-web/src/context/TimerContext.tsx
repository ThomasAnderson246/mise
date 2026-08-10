import { createContext, useContext, useState, useEffect, useRef } from "react";
import type { ReactNode } from "react";

export interface ActiveTimer {
  timerId: string;
  stepId: string;
  recipeTitle: string;
  stepInstruction: string;
  totalSeconds: number;
  remainingSeconds: number;
  startedAt: number;
  isComplete: boolean;
}

interface TimerContextType {
  timers: ActiveTimer[];
  startTimer: (
    stepId: string,
    recipeTitle: string,
    stepInstruction: string,
    durationMinutes: number,
  ) => string;
  pauseTimer: (timerId: string) => void;
  resumeTimer: (timerId: string) => void;
  dismissTimer: (timerId: string) => void;
  hasActiveTimers: boolean;
}

const TimerContext = createContext<TimerContextType | null>(null);

export function TimerProvider({ children }: { children: ReactNode }) {
  const [timers, setTimers] = useState<ActiveTimer[]>([]);
  const intervalRef = useRef<ReturnType<typeof setInterval> | null>(null);

  useEffect(() => {
    intervalRef.current = setInterval(() => {
      setTimers((prev) =>
        prev.map((timer) => {
          if (timer.isComplete || timer.remainingSeconds <= 0) {
            return { ...timer, remainingSeconds: 0, isComplete: true };
          }
          return { ...timer, remainingSeconds: timer.remainingSeconds - 1 };
        }),
      );
    }, 1000);

    return () => {
      if (intervalRef.current) clearInterval(intervalRef.current);
    };
  }, []);

  function startTimer(
    stepId: string,
    recipeTitle: string,
    stepInstruction: string,
    durationMinutes: number,
  ): string {
    const timerId = crypto.randomUUID();
    const totalSeconds = durationMinutes * 60;

    const newTimer: ActiveTimer = {
      timerId,
      stepId,
      recipeTitle,
      stepInstruction,
      totalSeconds,
      remainingSeconds: totalSeconds,
      startedAt: Date.now(),
      isComplete: false,
    };

    setTimers((prev) => [...prev, newTimer]);
    return timerId;
  }

  function pauseTimer(timerId: string) {
    // this needs to be cleanedup later... it works for now
    setTimers((prev) =>
      prev.map((t) => (t.timerId === timerId ? { ...t, isComplete: true } : t)),
    );
  }

  function resumeTimer(timerId: string) {
    setTimers((prev) =>
      prev.map((t) =>
        t.timerId === timerId ? { ...t, isComplete: false } : t,
      ),
    );
  }

  function dismissTimer(timderId: string) {
    setTimers((prev) => prev.filter((t) => t.timerId !== timderId));
  }

  const hasActiveTimers = timers.length > 0;

  return (
    <TimerContext.Provider
      value={{
        timers,
        startTimer,
        pauseTimer,
        resumeTimer,
        dismissTimer,
        hasActiveTimers,
      }}
    >
      {children}
    </TimerContext.Provider>
  );
}

export function useTimers() {
  const context = useContext(TimerContext);
  if (!context) throw new Error("useTimers must be used within TimerProvider");
  return context;
}
