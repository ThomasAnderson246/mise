import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { getPrepLists, createPrepList } from "@/api/prepListApi";
import { PageHeader } from "@/components/PageHeader";
import { EmptyState } from "@/components/EmptyState";
import { Button } from "@/components/ui/button";
import { toast } from "sonner";
import type { PrepList } from "@/api/prepListApi";

export default function PrepListPage() {
  const { user, hasPermission } = useAuth();
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();

  //state variables
  const [prepLists, setPrepLists] = useState<PrepList[]>([]);
  const [loading, setLoading] = useState(true);
  const [showCreateform, setShowCreateForm] = useState(false);
  const [newName, setNewName] = useState("");
  const [creating, setCreating] = useState(false);
  const [filter, setFilter] = useState<"all" | "active" | "complete">("active");

  useEffect(() => {
    if (!user?.token) return;

    getPrepLists(user.token)
      .then(setPrepLists)
      .catch(() => toast.error("Failed to load prep lists."))
      .finally(() => setLoading(false));
  }, [user]);

  async function handleCreate() {
    if (!user?.token || !newName.trim()) return;

    setCreating(true);

    try {
      const created = await createPrepList(user.token, {
        name: newName,
        assignedTo: null,
      });
      setPrepLists((prev) => [created, ...prev]);
      setNewName("");
      setShowCreateForm(false);
      toast.success("Prep list created.");
    } catch {
      toast.error("Failed to create prep list.");
    } finally {
      setCreating(false);
    }
  }

  const filtered = prepLists.filter((pl) => {
    if (filter === "active") return !pl.isComplete;
    if (filter === "complete") return pl.isComplete;

    return true;
  });

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground"> Loading prep lists...</p>
      </div>
    );
  }

  return (
    <div>
      <PageHeader
        title="Prep Lists"
        subtitle={`${prepLists.filter((pl) => !pl.isComplete).length} active`}
        action={
          hasPermission("preplist", "create") ? (
            <Button
              onClick={() => setShowCreateForm(true)}
              className="bg-primary text-primary-foreground"
            >
              New Prep List
            </Button>
          ) : undefined
        }
      />

      {showCreateform && (
        <div className="mb-6 p-4 bg-card rounded-lg border border-border">
          <p className="text-sm font-medium text-foreground mb-3">
            New prep list
          </p>
          <div className="flex gap-2">
            <input
              type="text"
              value={newName}
              onChange={(e) => setNewName(e.target.value)}
              placeholder="Prep list name..."
              onKeyDown={(e) => e.key === "Enter" && handleCreate()}
              className="flex-1 px-4 py-2.5 rounded-lg border border-border"
              autoFocus
            />
            <Button
              onClick={handleCreate}
              disabled={creating || !newName.trim()}
              className="bg-primary text-primary-foreground"
            >
              {creating ? "Creating..." : "Create"}
            </Button>
            <Button
              variant="outline"
              onClick={() => {
                setShowCreateForm(false);
                setNewName("");
              }}
            >
              Cancel
            </Button>
          </div>
        </div>
      )}

      <div className="flex gap-2 mb-6">
        {(["active", "all", "complete"] as const).map((f) => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={`text-sm px-4 py-2 rounded-lg border transition-colors ${
              filter === f
                ? "bg-primary text-primary-foreground border-primary"
                : "bg-card text-foreground border-border hover:border-primary"
            }`}
          >
            {f.charAt(0).toUpperCase() + f.slice(1)}
          </button>
        ))}
      </div>

      {filtered.length === 0 ? (
        <EmptyState
          title={filter === "active" ? "No active prep lists" : "No prep lists"}
          description={
            filter === "active"
              ? "All caught up!"
              : "Create a prep list to get started."
          }
          action={
            hasPermission("preplist", "create") ? (
              <Button
                onClick={() => setShowCreateForm(true)}
                className="bg-primary text-primary-foreground"
              >
                New Prep List
              </Button>
            ) : undefined
          }
        />
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {filtered.map((pl) => {
            const total = pl.items.length;
            const completed = pl.items.filter((i) => i.isComplete).length;
            const progress = total > 0 ? (completed / total) * 100 : 0;

            return (
              <div
                key={pl.prepListId}
                onClick={() => navigate(`/${slug}/prep-lists/${pl.prepListId}`)}
                className="bg-card rounded-lg p-4 border border-border cursor-pointer hover:border-primary transition-colors"
              >
                <div className="flex items-start justify-between gap-2 mb-2">
                  <h3 className="font-medium text-foreground text-sm">
                    {pl.name}
                  </h3>
                  {pl.isComplete && (
                    <span className="text-xs px-2 py-0.5 rounded-full bg-green-100 text-green-800 font-medium flex-shrink-0">
                      Complete
                    </span>
                  )}
                </div>
                {pl.createdbyName && (
                  <p className="text-xs text-muted-foreground mb-1">
                    Created by {pl.createdbyName}
                  </p>
                )}
                {pl.assignedToName && (
                  <p className="text-xs text-secondary mb-2">
                    Assigned to {pl.assignedToName}
                  </p>
                )}
                <div className="flex items-center gap-3 mt-3">
                  <div className="flex-1 bg-muted rounded-full h-1.5">
                    <div
                      className="bg-secondary h-1.5 rounded-full transition-all"
                      style={{ width: `${progress}%` }}
                    />
                  </div>
                  <span className="text-xs text-muted-foreground whitespace-nowrap">
                    {completed} / {total}
                  </span>
                </div>
                <p className="text-xs text-muted-foreground mt-2">
                  {new Date(pl.createdAt).toLocaleDateString()}
                </p>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
