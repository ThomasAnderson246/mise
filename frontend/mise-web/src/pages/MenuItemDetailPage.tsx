import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import {
  getMenuItemById,
  publishMenuItem,
  resolveAllergens,
  removeManualAllergen,
} from "@/api/menuItemApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import { AllergenSection } from "@/components/menuitem/AllergenSection";
import { LinkedRecipesList } from "@/components/menuitem/LinkedRecipesList";
import { MenuItemStatusBadge } from "@/components/menuitem/MenuItemStatusBadge";
import { toast } from "sonner";
import type { MenuItem } from "@/api/menuItemApi";

export default function MenuItemDetailPage() {
  const { user, hasPermission } = useAuth();
  const { slug, menuItemId } = useParams<{
    slug: string;
    menuItemId: string;
  }>();
  const navigate = useNavigate();

  const [menuItem, setMenuItem] = useState<MenuItem | null>(null);
  const [loading, setLoading] = useState(true);
  const [resolving, setResolving] = useState(false);
  const [publishing, setPublishing] = useState(false);
  const [showPublishConfirm, setShowPublishConfirm] = useState(false);

  useEffect(() => {
    if (!user?.token || !menuItemId) return;
    getMenuItemById(user.token, menuItemId)
      .then(setMenuItem)
      .catch(() => {
        toast.error("Failed to load menu item.");
        navigate(`/${slug}/menu-items`);
      })
      .finally(() => setLoading(false));
  }, [user, menuItemId]);

  async function handleResolveAllerens() {
    if (!user?.token || !menuItemId) return;

    try {
      const updated = await resolveAllergens(user.token, menuItemId);
      setMenuItem(updated);
      toast.success("Allergens resolved.");
    } catch {
      toast.error("Failed to resolve allergens.");
    } finally {
      setResolving(false);
    }
  }

  async function handleRemoveAllergens(allergenId: string) {
    if (!user?.token || !menuItemId) return;
    try {
      const updated = await removeManualAllergen(
        user.token,
        menuItemId,
        allergenId,
      );
      setMenuItem(updated);
      toast.success("Allergen removed.");
    } catch {
      toast.error("Failed to remove allergen.");
    }
  }

  async function handlePublish() {
    if (!user?.token || !menuItemId) return;
    setPublishing(true);
    try {
      const updated = await publishMenuItem(user.token, menuItemId);
      setMenuItem(updated);
      setShowPublishConfirm(false);
      toast.success("Menu item published.");
    } catch {
      toast.error("Failed to publish menu item.");
    } finally {
      setPublishing(false);
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading...</p>
      </div>
    );
  }

  if (!menuItem) return null;

  const canManage = hasPermission("menuitem", "update");

  return (
    <div className="max-w-2xl">
      <PageHeader
        title={menuItem.name}
        subtitle={menuItem.course ?? undefined}
        action={
          <div className="flex gap-2">
            {canManage && (
              <Button
                variant="outline"
                onClick={() =>
                  navigate(`/${slug}/menu-items/${menuItemId}/edit`)
                }
              >
                Edit
              </Button>
            )}
            {canManage && menuItem.status === "draft" && (
              <>
                {showPublishConfirm ? (
                  <div className="flex items-center gap-2">
                    <span className="text-sm text-muted-foreground">
                      Publish?
                    </span>
                    <Button
                      onClick={handlePublish}
                      disabled={publishing}
                      className="bg-primary text-primary-foreground"
                    >
                      {publishing ? "Publishing..." : "Confirm"}
                    </Button>
                    <Button
                      variant="outline"
                      onClick={() => setShowPublishConfirm(false)}
                    >
                      Cancel
                    </Button>
                  </div>
                ) : (
                  <Button
                    onClick={() => setShowPublishConfirm(true)}
                    className="bg-primary text-primary-foreground"
                  >
                    Publish
                  </Button>
                )}
              </>
            )}
          </div>
        }
      />

      <div className="flex items-center gap-3 mb-6">
        <MenuItemStatusBadge status={menuItem.status} />
        {menuItem.createdByName && (
          <span className="text-xs text-muted-foreground">
            Added by {menuItem.createdByName}
          </span>
        )}
      </div>

      {menuItem.description && (
        <p className="text-sm text-muted-foreground mb-6">
          {menuItem.description}
        </p>
      )}

      <AllergenSection
        allergens={menuItem.allergens}
        canManage={canManage}
        menuItemId={menuItemId!}
        resolving={resolving}
        onResolve={handleResolveAllerens}
        onRemove={handleRemoveAllergens}
      />

      <LinkedRecipesList
        recipes={menuItem.recipes}
        canManage={canManage}
        menuItemId={menuItemId!}
      />
    </div>
  );
}
