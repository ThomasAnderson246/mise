import { useNavigate, useParams } from "react-router-dom";
import { Button } from "../ui/button";
import { MenuItemStatusBadge } from "./MenuItemStatusBadge";
import type { MenuItemRecipe } from "@/api/menuItemApi";

interface LinkedRecipesListProps {
  recipes: MenuItemRecipe[];
  canManage: boolean;
  menuItemId: string;
}

export function LinkedRecipesList({
  recipes,
  canManage,
  menuItemId,
}: LinkedRecipesListProps) {
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();

  return (
    <section>
      <div className="flex items-center justify-between mb-4 pb-2 border-b border-border ">
        <h2 className="text-lg font-semibold text-foreground">
          Linked recipes
        </h2>
        {canManage && (
          <Button
            variant="outline"
            onClick={() => navigate(`/${slug}/menu-items/${menuItemId}/edit`)}
            className="text-xs h-8 px-3"
          >
            Manage recipes
          </Button>
        )}
      </div>
      {recipes.length === 0 ? (
        <p className="text-sm text-muted-foreground">No recipes linked.</p>
      ) : (
        <ul className="space-y-2">
          {recipes.map((r) => (
            <li
              key={r.menuItemRecipeId}
              className="flex items-center gap-3 p-3 bg-card rounded-lg border border-border"
            >
              <div className="flex-1">
                <p className="text-sm font-medium text-foreground">
                  {r.recipeTitle}
                </p>
                {r.note && (
                  <p className="text-xs text-muted-foreground mt-0.5">
                    {r.note}
                  </p>
                )}
              </div>
              <MenuItemStatusBadge status={r.recipeStatus} />
              <button
                onClick={() => navigate(`/${slug}/recipes/${r.recipeId}`)}
                className="text-xs text-secondary hover:underline"
              >
                View
              </button>
            </li>
          ))}
        </ul>
      )}
    </section>
  );
}
