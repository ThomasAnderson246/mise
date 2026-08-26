import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import {
  getCategories,
  createCategory,
  deleteCategory,
} from "@/api/categoryApi";
import { Button } from "../ui/button";
import { toast } from "sonner";
import { inputClass } from "@/lib/styles";
import type { CategoryItem } from "@/api/categoryApi";

export function CategoriesTab() {
  const { user, hasPermission } = useAuth();
  const [categories, setCategories] = useState<CategoryItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [newName, setNewName] = useState("");
  const [creating, setCreating] = useState(false);

  useEffect(() => {
    if (!user?.token) return;
    getCategories(user.token)
      .then(setCategories)
      .catch(() => toast.error("Failed to load categories."))
      .finally(() => setLoading(false));
  }, [user]);

  async function handleCreate() {
    if (!user?.token || !newName.trim()) return;
    setCreating(true);
    try {
      const created = await createCategory(user.token, newName);
      setCategories((prev) => [...prev, created]);
      setNewName("");
      toast.success("Category created.");
    } catch {
      toast.error("Failed to create category.");
    } finally {
      setCreating(false);
    }
  }

  async function handleDelete(categoryId: string) {
    if (!user?.token) return;
    try {
      await deleteCategory(user.token, categoryId);
      setCategories((prev) => prev.filter((c) => c.categoryId !== categoryId));
      toast.success("Category deleted.");
    } catch {
      toast.error("Failed to delete category.");
    }
  }

  if (loading)
    return <p className="text-sm text-muted-foreground">Loading...</p>;

  return (
    <div className="max-w-lg space-y-4">
      {hasPermission("category", "create") && (
        <div className="flex gap-2">
          <input
            type="text"
            value={newName}
            onChange={(e) => setNewName(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleCreate()}
            placeholder="New category name..."
            className={inputClass}
            autoFocus
          />
          <Button
            onClick={handleCreate}
            disabled={creating || !newName.trim()}
            className="bg-primary text-primary-foreground"
          >
            {creating ? "Adding..." : "Add"}
          </Button>
        </div>
      )}
      <div className="space-y-2">
        {categories.length === 0 ? (
          <p className="text-sm text-muted-foreground">No categories yet.</p>
        ) : (
          categories.map((cat) => (
            <div
              key={cat.categoryId}
              className="flex items-center gap-3 p-3 bg-card rounded-lg border border-border"
            >
              <span className="flex-1 text-sm text-foreground">{cat.name}</span>
              {hasPermission("category", "delete") && (
                <button
                  onClick={() => handleDelete(cat.categoryId)}
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
  );
}
