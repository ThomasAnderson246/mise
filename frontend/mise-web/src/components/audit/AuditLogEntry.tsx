import { useState } from "react";
import {
  getHumanReadable,
  formatRelativeTime,
  getActionGroup,
  getActionGroupColor,
} from "@/config/auditLog";
import type { AuditLogEntry as AuditLogEntryType } from "@/api/auditLogApi";

interface AuditLogEntryProps {
  log: AuditLogEntryType;
}

export function AuditLogEntry({ log }: AuditLogEntryProps) {
  const [expanded, setExpanded] = useState(false);
  const hasDetails = !!(log.previousState || log.newState);
  const group = getActionGroup(log.action);

  function tryParseJson(str: string | null): string {
    if (!str) return "";
    try {
      return JSON.stringify(JSON.parse(str), null, 2);
    } catch {
      return str;
    }
  }

  return (
    <div className="bg-card rounded-lg border border-border overflow-hidden">
      <div
        className={`flex items-center gap-3 p-3 ${hasDetails ? "cursor-pointer hover:bg-muted transition-colors" : ""}`}
        onClick={() => hasDetails && setExpanded((prev) => !prev)}
      >
        <div className="w-8 h-8 rounded-full bg-primary flex items-center justify-cneter text-primary-foreground text-xs font-medium flex-shrink-0">
          {log.performedByName
            ? log.performedByName
                .split(" ")
                .map((n) => n[0])
                .join("")
            : "?"}
        </div>

        <div className="flex-1 min-w-0">
          <p className="text-sm text-foreground">
            {getHumanReadable(log.performedByName, log.action, log.resource)}
          </p>
          <div className="flex items-center gap-2 mt-0.5">
            <span
              className={`text-xs px-1.5 py-0.5 rounded font-medium ${getActionGroupColor(group)}`}
            >
              {log.action}
            </span>
            <span className="text-xs text-muted-foreground">
              {log.resource}
            </span>
          </div>
        </div>

        <div className="flex items-center gap-2 flex-shrink-0">
          <span
            className="text-xs text-muted-foreground"
            title={new Date(log.performedAt).toLocaleDateString()}
          >
            {formatRelativeTime(log.performedAt)}
          </span>
          {hasDetails && (
            <span className="text-muted-foreground text-xs">
              {expanded ? "up" : "down"}
            </span>
          )}
        </div>
      </div>

      {expanded && hasDetails && (
        <div className="border-t border-border p-3 space-y-3 bg-muted/50">
          {log.previousState && (
            <div>
              <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
                Before
              </p>
              <pre className="text-xs text-foreground bg-card rounded p-2 border border-border overflow-x-auto">
                {tryParseJson(log.previousState)}
              </pre>
            </div>
          )}
          {log.newState && (
            <div>
              <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-1">
                After
              </p>
              <pre className="text-xs text-foreground bg-card rounded p-2 border border-border overflow-x-auto">
                {tryParseJson(log.newState)}
              </pre>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
