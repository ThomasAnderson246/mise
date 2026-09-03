import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { getRecipes } from "@/api/recipeApi";
import { PageHeader } from "@/components/PageHeader";
import { EmptyState } from "@/components/EmptyState";
import { Button } from "@/components/ui/button";
import { getCategories } from "@/api/categoryApi";
import type { RecipeItem } from "@/api/recipeApi";
import type { CategoryItem } from "@/api/categoryApi";

export default function RecipesPage() {
  const { user, hasPermission } = useAuth();
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();

  const [recipes, setRecipes] = useState<RecipeItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [categoryFilter, setCategoryFilter] = useState<string>("all");
  const [categories, setCategories] = useState<CategoryItem[]>([]);
  const [view, setView] = useState<"recipes" | "portions">("recipes");

  useEffect(() => {
    if (!user?.token) return;

    getRecipes(user.token)
      .then(setRecipes)
      .catch((err) => console.error("Failed to load recipes:", err))
      .finally(() => setLoading(false));

    getCategories(user.token)
      .then(setCategories)
      .catch((err) => console.error("Failed to load categories:", err));
  }, [user]);

  const filtered = recipes.filter((r) => {
    const matchesSearch = r.title.toLowerCase().includes(search.toLowerCase());
    const matchesCategory =
      categoryFilter === "all"
        ? true
        : categoryFilter === "uncategorized"
          ? r.recipeCategories?.length === 0
          : r.recipeCategories?.some((rc) => rc.categoryId === categoryFilter);
    const matchesDraftFilter =
      hasPermission("recipe", "create") ||
      hasPermission("recipe", "update") ||
      r.status === "published";
    const matchesView = view === "portions" ? r.isPortion : !r.isPortion;
    return (
      matchesCategory && matchesSearch && matchesDraftFilter && matchesView
    );
  });

  function getStatusColor(status: string) {
    switch (status) {
      case "published":
        return "bg-green-100 text-green-800";
      case "draft":
        return "bg-yellow-100 text-yellow-800";
      default:
        return "bg-muted text-muted-foreground";
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading recipes...</p>
      </div>
    );
  }

  return (
    <div>
      <PageHeader
        title="Recipes"
        subtitle={
          view === "portions"
            ? `${filtered.length} portion size${filtered.length} !== 1 ? 's' : ''}`
            : `${recipes.filter((r) => !r.isPortion).length} recipe${recipes.filter((r) => !r.isPortion).length !== 1 ? "s" : ""} in your book`
        }
        action={
          hasPermission("recipe", "create") ? (
            <Button
              onClick={() => navigate(`/${slug}/recipes/new`)}
              className="bg-primary text-primary-foreground"
            >
              {view === "portions" ? "New Portion" : "New Recipe"}
            </Button>
          ) : undefined
        }
      />

      <div className="flex gap-2 mb-6">
        {(["recipes", "portions"] as const).map((v) => (
          <button
            key={v}
            onClick={() => {
              setView(v);
              setCategoryFilter("all");
              setSearch("");
            }}
            className={`text-sm px-4 py-2 rounded-lg border transition-colors ${
              view === v
                ? "bg-primary text-primary-foreground border-primary"
                : "bg-card text-foreground border-border hover:border-primary"
            }`}
          >
            {v === "recipes" ? "Recipes" : "Portion Sizes"}
          </button>
        ))}
      </div>

      {view === "recipes" && (
        <div className="mb-6">
          <input
            type="text"
            placeholder="Search recipes..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full md:w-80 px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring placeholder:text-muted-foreground"
          />
          <select
            value={categoryFilter}
            onChange={(e) => setCategoryFilter(e.target.value)}
            className="px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            <option value="all">All Categories</option>
            <option value="uncategorized">Uncategorized</option>
            {categories.map((cat) => (
              <option key={cat.categoryId} value={cat.categoryId}>
                {cat.name}
              </option>
            ))}
          </select>
        </div>
      )}

      {view === "portions" && (
        <div className="mb-6">
          <input
            type="text"
            placeholder="Search portions..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full md:w-80 px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring placeholder:text-muted-foreground"
          />
        </div>
      )}

      {filtered.length === 0 ? (
        <EmptyState
          title={
            search
              ? `No ${view === "portions" ? "portions" : "recipes"} match your search`
              : view === "portions"
                ? "No portion sizes yet"
                : "No recipes yet"
          }
          description={
            search
              ? "Try a different search term."
              : view === "portions"
                ? "Add your first portion size to get started."
                : "Add your first recipe to get started."
          }
          action={
            hasPermission("recipe", "create") && !search ? (
              <Button
                onClick={() =>
                  navigate(
                    `/${slug}/recipes/new?isPortion=${view === "portions"}`,
                  )
                }
                className="bg-primary text-primary-foreground hover:bg-primary/90"
              >
                {view === "portions" ? "New Portion" : "New Recipe"}
              </Button>
            ) : undefined
          }
        />
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {filtered.map((recipe) => {
            console.log("view:", view, "isPortion:", recipe.isPortion);
            return (
              <div
                key={recipe.recipeId}
                onClick={() => navigate(`/${slug}/recipes/${recipe.recipeId}`)}
                className="bg-card rounded-lg p-4 border border-border cursor-pointer hover:border-primary transition-colors"
              >
                <div className="flex items-start justify-between gap-2 mb-2">
                  <h3 className="font-medium text-foreground text-sm leading-snug">
                    {recipe.title}
                  </h3>
                  {recipe.hasActiveDraft && (
                    <span className="text-xs px-2 py-0.5 rounded-full font-medium bg-yellow-100 text-yellow-800">
                      draft pending
                    </span>
                  )}

                  {recipe.status === "published" && (
                    <span
                      className={`text-xs px-2 py-0.5 rounded-full font-medium flex-shrink-0 ${getStatusColor(recipe.status)}`}
                    >
                      {recipe.status}
                    </span>
                  )}
                </div>
                {view === "portions" ? (
                  <p className="text-xs text-muted-foreground mt-1">
                    {recipe.ingredientCount} ingredient
                    {recipe.ingredientCount !== 1 ? "s" : ""}
                  </p>
                ) : (
                  recipe.description && (
                    <p className="text-xs text-muted-foreground line-clamp-2 mb-3">
                      {recipe.description}
                    </p>
                  )
                )}
                <p className="text-xs text-muted-foreground">
                  Updated {new Date(recipe.updatedAt).toLocaleDateString()}
                </p>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
