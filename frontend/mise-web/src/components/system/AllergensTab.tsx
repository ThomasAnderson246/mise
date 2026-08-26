import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import {
  getAllAllergens,
  createAllergenTag,
  deleteAllergenTag,
} from "@/api/allergenApi";
import { Button } from "../ui/button";
import { toast } from "sonner";
import { inputClass } from "@/lib/styles";
import type { AllergenTagItem } from "@/api/allergenApi";

export function AllergensTab() {
  const { user, hasPermission } = useAuth();
  const [allergens, setAllergens] = useState<AllergenTagItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [newName, setNewName] = useState("");
  const [newDescription, setNewDescription] = useState("");
  const [isMajor, setIsMajor] = useState(false);
  const [creating, setCreating] = useState(false);
  const [showForm, setShowForm] = useState(false);

  useEffect(() => {
    if (!user?.token) return;
    getAllAllergens(user.token)
      .then(setAllergens)
      .catch(() => toast.error("Failed to load allergens."))
      .finally(() => setLoading(false));
  }, [user]);

  async function handleCreate() {
    if (!user?.token || !newName.trim()) return;
    setCreating(true);
    try {
      const created = await createAllergenTag(user.token, {
        name: newName,
        description: newDescription,
        isMajor,
      });
      setAllergens((prev) => [...prev, created]);
      setNewName("");
      setNewDescription("");
      setIsMajor(false);
      setShowForm(false);
      toast.success("Allergen created.");
    } catch {
      toast.error("Failed to create allergen.");
    } finally {
      setCreating(false);
    }
  }

  async function handleDelete(allergenId: string) {
    if (!user?.token) return;
    try {
      await deleteAllergenTag(user.token, allergenId);
      setAllergens((prev) => prev.filter((a) => a.allergenId !== allergenId));
      toast.success("Allergen deleted.");
    } catch {
      toast.error("Failed to delete allergen. It may be in use.");
    }
  }

  if (loading)
    return <p className="text-sm text-muted-foreground">Loading...</p>;

  const systemAllergens = allergens.filter((a) => a.isSystemDefined);
  const customAllergens = allergens.filter((a) => !a.isSystemDefined);

  return (
    <div className="max-w-lg space-y-6">
      <div>
        <h3 className="text-sm font-medium text-muted-foreground uppercase tracking-wide mb-3">
          System allergens
        </h3>
        <div className="space-y-2">
          {systemAllergens.map((a) => (
            <div
              key={a.allergenId}
              className="flex items-center gap-3 p-3 bg-card rounded-lg border border-border"
            >
              <div className="flex-1">
                <p className="text-sm text-foreground">{a.name}</p>
                {a.description && (
                  <p className="text-xs text-muted-foreground">
                    {a.description}
                  </p>
                )}
              </div>
              {a.isMajor && (
                <span className="text-xs px-2 py-0.5 rounded-full bg-red-100 text-red-800">
                  major
                </span>
              )}
            </div>
          ))}
        </div>
      </div>

      <div>
        <h3 className="text-sm font-medium text-muted-foreground uppercase tracking-wide mb-3">
          Custom allergens
        </h3>
        <div className="space-y-2 mb-4">
          {customAllergens.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              No custom allergens yet.
            </p>
          ) : (
            customAllergens.map((a) => (
              <div
                key={a.allergenId}
                className="flex items-center gap-3 p-3 bg-card rounded-lg border border-border"
              >
                <div className="flex-1">
                  <p className="text-sm text-foreground">{a.name}</p>
                  {a.description && (
                    <p className="text-xs text-muted-foreground">
                      {a.description}
                    </p>
                  )}
                </div>
                {a.isMajor && (
                  <span className="text-xs px-2 py-0.5 rounded-full bg-red-100 text-red-800">
                    major
                  </span>
                )}
                {hasPermission("allergen", "delete") && (
                  <button
                    onClick={() => handleDelete(a.allergenId)}
                    className="text-xs text-destructive hover:underline"
                  >
                    Delete
                  </button>
                )}
              </div>
            ))
          )}
        </div>
      </div>

      {hasPermission("allergen", "create") &&
        (!showForm ? (
          <Button variant="outline" onClick={() => setShowForm(true)}>
            + Add allergen
          </Button>
        ) : (
          <div className="p-4 bg-card rounded-lg border border-border space-y-3">
            <input
              type="text"
              value={newName}
              onChange={(e) => setNewName(e.target.value)}
              placeholder="Allergen name..."
              className={inputClass}
              autoFocus
            />

            <label className="flex items-center gap-2 text-sm text-foreground">
              <input
                type="checkbox"
                checked={isMajor}
                onChange={(e) => setIsMajor(e.target.checked)}
              />
            </label>
            <div className="flex gap-2">
              <Button
                onClick={handleCreate}
                disabled={creating || !newName.trim()}
                className="bg-primary text-primary-foreground"
              >
                {creating ? "Adding..." : "Add"}
              </Button>
              <Button
                variant="outline"
                onClick={() => {
                  setShowForm(false);
                  setNewName("");
                  setNewDescription("");
                  setIsMajor(false);
                }}
              >
                Cancel
              </Button>
            </div>
          </div>
        ))}
    </div>
  );
}
