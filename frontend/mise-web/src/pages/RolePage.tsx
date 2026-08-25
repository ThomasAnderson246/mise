import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import {
  getRoles,
  getAllPermissions,
  addPermissionToRole,
  removePermissionFromRole,
  createRole,
} from "@/api/roleApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import { RoleCard } from "@/components/roles/RoleCard";
import { toast } from "sonner";
import { inputClass } from "@/lib/styles";
import type { Role, Permission } from "@/api/roleApi";

export default function RolesPage() {
  const { user } = useAuth();

  const [roles, setRoles] = useState<Role[]>([]);
  const [allPermissions, setAllPermissions] = useState<Permission[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [newRoleName, setNewRoleName] = useState("");
  const [creating, setCreating] = useState(false);

  async function loadData() {
    if (!user?.token) return;
    try {
      const [rolesData, permissionData] = await Promise.all([
        getRoles(user.token),
        getAllPermissions(user.token),
      ]);
      console.log("Permission Data: ", permissionData);
      setRoles(rolesData);
      setAllPermissions(permissionData);
      console.log("All permissions:", allPermissions);
    } catch {
      toast.error("Failed to load roles.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadData();
  }, [user]);

  async function handleAddPermission(roleId: string, permissionId: string) {
    if (!user?.token) return;
    try {
      await addPermissionToRole(user.token, roleId, permissionId);
      setRoles((prev) =>
        prev.map((r) =>
          r.roleId === roleId
            ? {
                ...r,
                permissions: [
                  ...r.permissions,
                  allPermissions.find((p) => p.permissionId === permissionId)!,
                ],
              }
            : r,
        ),
      );
      toast.success("Permission added.");
    } catch {
      toast.error("Failed to add permission.");
    }
  }
  async function handleRemovePermission(roleId: string, permissionId: string) {
    if (!user?.token) return;
    try {
      await removePermissionFromRole(user.token, roleId, permissionId);
      setRoles((prev) =>
        prev.map((r) =>
          r.roleId === roleId
            ? {
                ...r,
                permissions: r.permissions.filter(
                  (p) => p.permissionId !== permissionId,
                ),
              }
            : r,
        ),
      );
      toast.success("Permission removed.");
    } catch {
      toast.error("Failed to remove permission.");
    }
  }

  async function handleCreateRole() {
    if (!user?.token || !newRoleName.trim()) return;
    setCreating(true);
    try {
      const created = await createRole(user.token, newRoleName);
      setRoles((prev) => [...prev, created]);
      setNewRoleName("");
      setShowCreateForm(false);
      toast.success("Role created.");
    } catch {
      toast.error("Failed to create role.");
    } finally {
      setCreating(false);
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading roles...</p>
      </div>
    );
  }

  return (
    <div className="max-w-2xl">
      <PageHeader
        title="Roles"
        subtitle={`${roles.length} roles`}
        action={
          <Button
            onClick={() => setShowCreateForm(true)}
            className="bg-primary text-primary-foreground"
          >
            New role
          </Button>
        }
      />

      {showCreateForm && (
        <div className="mb-6 p-4 bg-card rounded-lg border border-border">
          <p className="text-sm font-medium text-foreground mb-3">New role</p>
          <div className="flex gap-2">
            <input
              type="text"
              value={newRoleName}
              onChange={(e) => setNewRoleName(e.target.value)}
              placeholder="Role name..."
              onKeyDown={(e) => e.key === "Enter" && handleCreateRole()}
              className={inputClass}
              autoFocus
            />
            <Button
              onClick={handleCreateRole}
              disabled={creating || !newRoleName.trim()}
              className="bg-primary text-primary-foreground"
            >
              {creating ? "Creating..." : "Create"}
            </Button>
            <Button
              variant="outline"
              onClick={() => {
                setShowCreateForm(false);
                setNewRoleName("");
              }}
            >
              Cancel
            </Button>
          </div>
        </div>
      )}

      <div className="space-y-2">
        {roles.map((role) => (
          <RoleCard
            key={role.roleId}
            role={role}
            allPermissions={allPermissions}
            onAddPermission={handleAddPermission}
            onRemovePermission={handleRemovePermission}
          />
        ))}
      </div>
    </div>
  );
}
