import { useState } from "react";
import { Button } from "../ui/button";
import type { Role, Permission } from "@/api/roleApi";

interface RoleCardProps {
  role: Role;
  allPermissions: Permission[];
  onAddPermission: (roleId: string, permissionsId: string) => Promise<void>;
  onRemovePermission: (roleId: string, permissionsId: string) => Promise<void>;
}

export function RoleCard({
  role,
  allPermissions,
  onAddPermission,
  onRemovePermission,
}: RoleCardProps) {
  const [expanded, setExpanded] = useState(false);
  const [adding, setAdding] = useState(false);
  const [selectedPermissionId, setSelectedPermissionId] = useState("");

  const assignedIds = new Set(role.permissions.map((p) => p.permissionId));
  const availableToAdd = allPermissions.filter(
    (p) => !assignedIds.has(p.permissionId),
  );

  // group permissions by their resource
  const grouped = role.permissions.reduce(
    (acc, p) => {
      if (!acc[p.resource]) acc[p.resource] = [];
      acc[p.resource].push(p);
      return acc;
    },
    {} as Record<string, typeof role.permissions>,
  );

  async function handleAdd() {
    if (!selectedPermissionId) return;
    await onAddPermission(role.roleId, selectedPermissionId);
    setSelectedPermissionId("");
    setAdding(false);
  }

  return (
    <div className="bg-card rounded-lg border border-border overflow-hidden">
      <button
        onClick={() => setExpanded((prev) => !prev)}
        className="w-full flex items-center justify-between p-4 text-left hover:bg-muted transition-colors"
      >
        <div className="flex items-center gap-3">
          <span className="text-sm font-medium text-foreground">
            {role.name}
          </span>
          {role.isSystemRole && (
            <span className="text-xs px-2 py-0.5 rounded-full bg-muted text-muted-foreground">
              system
            </span>
          )}
          <span className="text-xs text-muted-foreground">
            {role.permissions.length} permission
            {role.permissions.length !== 1 ? "s" : ""}
          </span>
        </div>
        <span className="text-muted-foreground">{expanded ? "▲" : "▼"}</span>
      </button>

      {expanded && (
        <div className="border-t border-border p-4 space-y-4">
          {role.permissions.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              No permissions assigned.
            </p>
          ) : (
            Object.entries(grouped)
              .sort()
              .map(([resource, perms]) => (
                <div key={resource}>
                  <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-2">
                    {resource}
                  </p>
                  <div className="flex flex-wrap gap2">
                    {perms.map((p) => (
                      <div
                        key={p.permissionId}
                        className="flex items-center gap-1"
                      >
                        <span className="text-xs px-2 py-1 rounded-lg bg-muted text-foreground">
                          {p.action}
                        </span>
                        <button
                          onClick={() =>
                            onRemovePermission(role.roleId, p.permissionId)
                          }
                          className="text-xs text-destructive hover:underline"
                        >
                          X
                        </button>
                      </div>
                    ))}
                  </div>
                </div>
              ))
          )}

          {!adding ? (
            <Button
              variant="outline"
              onClick={() => setAdding(true)}
              className="text-xs h-8 px-3"
            >
              + Add permission
            </Button>
          ) : (
            <div className="flex gap-2">
              <select
                value={selectedPermissionId}
                onChange={(e) => setSelectedPermissionId(e.target.value)}
                className="flex-1 px-3 py-2 rounded-lg border border-border bg-backround text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
              >
                <option value="">Select permission...</option>
                {Object.entries(
                  availableToAdd.reduce(
                    (acc, p) => {
                      if (!acc[p.resource]) acc[p.resource] = [];
                      acc[p.resource].push(p);
                      return acc;
                    },
                    {} as Record<string, Permission[]>,
                  ),
                )
                  .sort()
                  .map(([resource, perms]) => (
                    <optgroup key={resource} label={resource}>
                      {perms.map((p) => (
                        <option key={p.permissionId} value={p.permissionId}>
                          {p.action}
                        </option>
                      ))}
                    </optgroup>
                  ))}
              </select>
              <Button
                onClick={handleAdd}
                disabled={!selectedPermissionId}
                className="bg-primary text-primary-foreground text-xs h-9 px-3"
              >
                Add
              </Button>
              <Button
                variant="outline"
                onClick={() => {
                  setAdding(false);
                  setSelectedPermissionId("");
                }}
                className="text-xs h-9 px-3"
              >
                Cancel
              </Button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
