import { ACTION_GROUPS, ENTITY_TYPES, DATE_RANGES } from "@/config/auditLog";
import type { UserItem } from "@/api/userApi";

interface AuditLogFiltersProps {
  dateRange: string;
  entityType: string;
  selectedGroups: string[];
  selectedUser: string;
  users: UserItem[];
  onDateRangeChange: (value: string) => void;
  onEntityTypeChange: (value: string) => void;
  onGroupToggle: (group: string) => void;
  onUserChange: (value: string) => void;
}

export function AuditLogFilters({
  dateRange,
  entityType,
  selectedGroups,
  selectedUser,
  users,
  onDateRangeChange,
  onEntityTypeChange,
  onGroupToggle,
  onUserChange,
}: AuditLogFiltersProps) {
  return (
    <div className="bg-card rounded-lg border border-border p-4 mb-6 space-y-4">
      <div className="flex flex-col sm:flex-row gap-4">
        <div>
          <label className="block text-xs font-medium text-muted-foreground mb-1.5">
            Date range
          </label>
          <select
            value={dateRange}
            onChange={(e) => onDateRangeChange(e.target.value)}
            className="px-3 py-2 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            {DATE_RANGES.map((r) => (
              <option key={r.value} value={r.value}>
                {r.label}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-xs font-medium text-muted-foreground mb-1.5">
            Entity type
          </label>
          <select
            value={entityType}
            onChange={(e) => onEntityTypeChange(e.target.value)}
            className="px-3 py-2 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            {ENTITY_TYPES.map((t) => (
              <option key={t} value={t}>
                {t.charAt(0).toUpperCase() + t.slice(1).replace("_", " ")}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-xs font-medium text-muted-foreground mb-1.5">
            Performed by
          </label>
          <select
            value={selectedUser}
            onChange={(e) => onUserChange(e.target.value)}
            className="px-3 py-2 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            {users.map((u) => (
              <option key={u.userId} value={u.userId}>
                {u.firstName} {u.lastName}
              </option>
            ))}
          </select>
        </div>
      </div>

      <div>
        <label className="block text-xs font-medium text-muted-foreground mb-1.5">
          Actions
        </label>
        <div className="flex flex-wrap gap-3">
          {Object.keys(ACTION_GROUPS).map((group) => (
            <label
              key={group}
              className="flex items-center gap-2 text-sm text-foreground cursor-pointer"
            >
              <input
                type="checkbox"
                checked={selectedGroups.includes(group)}
                onChange={() => onGroupToggle(group)}
                className="accent-secondary"
              />
              {group.charAt(0).toUpperCase() + group.slice(1)}
            </label>
          ))}
        </div>
      </div>
    </div>
  );
}
