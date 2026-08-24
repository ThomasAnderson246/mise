import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { getUsers, deactivateUser, reactivateUser } from "@/api/userApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import { UserCard } from "@/components/users/UserCard";
import { InviteUserModal } from "@/components/users/InviteUserModal";
import { toast } from "sonner";
import type { UserItem } from "@/api/userApi";

export default function UsersPage() {
  const { user, hasPermission } = useAuth();
  const canManage = hasPermission("user", "manage");

  const [users, setUsers] = useState<UserItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [showInvite, setShowInvite] = useState(false);
  const [filter, setFilter] = useState<"all" | "active" | "inactive">("active");

  async function loadUsers() {
    if (!user?.token) return;
    try {
      const data = await getUsers(user.token);
      setUsers(data);
    } catch {
      toast.error("Failed to load users.");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    loadUsers();
  }, [user]);

  async function handleDeactivte(userId: string) {
    if (!user?.token) return;
    try {
      await deactivateUser(user.token, userId);
      setUsers((prev) =>
        prev.map((u) =>
          u.userId === userId ? { ...u, status: "inactive" } : u,
        ),
      );
      toast.success("User deactivated.");
    } catch {
      toast.error("Failed to deactivate user.");
    }
  }

  async function handleReactivate(userId: string) {
    if (!user?.token) return;

    try {
      await reactivateUser(user.token, userId);
      setUsers((prev) =>
        prev.map((u) => (u.userId === userId ? { ...u, status: "active" } : u)),
      );
      toast.success("User reactivated.");
    } catch {
      toast.error("Failed to reactivate user.");
    }
  }

  const filtered = users.filter((u) => {
    if (filter === "active")
      return u.status === "active" || u.status === "pending";
    if (filter === "inactive") return u.status === "inactive";
    return true;
  });

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground"> Loading users...</p>
      </div>
    );
  }

  return (
    <div className="max-w-2xl">
      <PageHeader
        title="Users"
        subtitle={`${users.filter((u) => u.status === "active").length} active`}
        action={
          <Button
            onClick={() => setShowInvite(true)}
            className="bg-primary text-primary-foreground"
          >
            Invite user
          </Button>
        }
      />

      <div className="flex gap-2 mb-6">
        {(["active", "all", "inactive"] as const).map((f) => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={`text-sm px-4 py-2 rounded-lg border transition-colors ${
              filter === f
                ? "bg-primary text-primary-foreground border-primary"
                : "bg-card text-foreground border-border hoer:border-primary"
            }`}
          >
            {f.charAt(0).toUpperCase() + f.slice(1)}
          </button>
        ))}
      </div>

      <div className="space-y-2">
        {filtered.length === 0 ? (
          <p className="text-sm text-muted-foreground text-center py-8">
            No users found.
          </p>
        ) : (
          filtered.map((u) => (
            <UserCard
              key={u.userId}
              user={u}
              currentUserId={user?.userId ?? ""}
              canManage={canManage}
              onDeactivate={handleDeactivte}
              onReactivate={handleReactivate}
            />
          ))
        )}
      </div>
      <InviteUserModal
        open={showInvite}
        onClose={() => setShowInvite(false)}
        onInvited={() => {
          loadUsers();
          toast.success("User invited successfully.");
        }}
      />
    </div>
  );
}
