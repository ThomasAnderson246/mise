import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import {
  getMenuItemById,
  createMenuItem,
  updateMenuItem,
  AddMenuItemRecipe,
  removeMenuItemRecipe,
} from "@/api/menuItemApi";
import { getRecipes } from "@/api/recipeApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import { LinkedRecipesEditor } from "@/components/menuitem/LinkedRecipesEditor";
import { toast } from "sonner";
import { inputClass } from "@/lib/styles";
import type { MenuItem } from "@/api/menuItemApi";
import type { RecipeItem } from "@/api/recipeApi";

export default function MenuItemEditorPage() {
  const { user } = useAuth();
  const { slug, menuItemId } = useParams<{
    slug: string;
    menuItemId?: string;
  }>();
  const navigate = useNavigate();
  const isEditMode = !menuItemId;

  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [course, setCourse] = useState("");
  const [menuItem, setMenuItem] = useState<MenuItem | null>(null);
  const [currentMenuItemId, setCurrentMenuItemId] = useState<string | null>(
    menuItemId ?? null,
  );
  const [recipes, setRecipes] = useState<RecipeItem[]>([]);
  const [loading, setLoading] = useState(isEditMode);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    async function load() {
      try {
        const recipeData = await getRecipes(user!.token);
        setRecipes(
          recipeData.filter((r) => r.status === "published" && !r.isPortion),
        );

        if (isEditMode && menuItemId) {
          const data = await getMenuItemById(user!.token, menuItemId);
          setMenuItem(data);
          setName(data.name);
          setDescription(data.description ?? "");
          setCourse(data.course ?? "");
        }
      } catch {
        toast.error("Failed to load data.");
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [user, menuItemId, isEditMode]);

  async function handleSave() {
    if (!user?.token || !name.trim()) return;
    setSaving(true);

    try {
      if (!currentMenuItemId) {
        const created = await createMenuItem(user.token, {
          name,
          description: description || null,
          course: course || null,
        });
        setCurrentMenuItemId(created.menuItemId);
        setMenuItem(created);
        navigate(`/${slug}/menu-items/${created.menuItemId}/edit`, {
          replace: true,
        });
        toast.success("Menu item created.");
      } else {
        const updated = await updateMenuItem(user.token, currentMenuItemId, {
          name,
          description: description || null,
          course: course || null,
        });
        setMenuItem(updated);
        toast.success("Menu item saved.");
      }
    } catch {
      toast.error("Failed to save menu item.");
    } finally {
      setSaving(false);
    }
  }

  async function handleAddRecipe(recipeId: string, note: string | null) {
    if (!user?.token || !currentMenuItemId) return;
    const updated = await AddMenuItemRecipe(user.token, currentMenuItemId, {
      recipeId,
      displayOrder: (menuItem?.recipes.length ?? 0) + 1,
      note,
    });
    setMenuItem(updated);
    toast.success("Recipe linked.");
  }

  async function handleRemoveRecipe(recipeId: string) {
    if (!user?.token || !currentMenuItemId) return;
    const updated = await removeMenuItemRecipe(
      user.token,
      currentMenuItemId,
      recipeId,
    );
    setMenuItem(updated);
    toast.success("Recipe removed.");
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading...</p>
      </div>
    );
  }

  return (
    <div className="max-w-2xl">
      <PageHeader
        title={isEditMode ? "Edit Menu Item" : "New Menu Item"}
        action={
          <div className="flex gap-2">
            <Button variant="outline" onClick={() => navigate(-1)}>
              Cancel
            </Button>
            <Button
              onClick={handleSave}
              disabled={saving || !name.trim()}
              className="bg-primary text-primary-foreground"
            >
              {saving ? "Saving..." : "Save"}
            </Button>
          </div>
        }
      />

      <div className="space-y-4 mb-8">
        <div>
          <label className="block text-sm font-medium text-foreground mb-1">
            Name
          </label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="Menu item name"
            className={inputClass}
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-foreground mb-1">
            Description
          </label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Brief description"
            rows={3}
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline focus:ring-2 focus:ring-ring resize-none"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-foreground mb-1">
            Course
          </label>
          <input
            type="text"
            value={course}
            onChange={(e) => setCourse(e.target.value)}
            placeholder="e.g. Appetizer, Main, Dessert..."
            className={inputClass}
          />
        </div>
      </div>

      {currentMenuItemId ? (
        <LinkedRecipesEditor
          recipes={menuItem?.recipes ?? []}
          availableRecipes={recipes}
          onAdd={handleAddRecipe}
          onRemove={handleRemoveRecipe}
        />
      ) : (
        <div className="bg-card rounded-lg p-6 border border-border text-center">
          <p className="text-sm text-muted-foreground">
            Save the menu item details above to start linking recipes.
          </p>
        </div>
      )}
    </div>
  );
}
