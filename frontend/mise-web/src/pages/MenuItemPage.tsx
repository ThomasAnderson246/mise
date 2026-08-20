import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { getMenuItems } from "@/api/menuItemApi";
import { PageHeader } from "@/components/PageHeader";
import { EmptyState } from "@/components/EmptyState";
import { Button } from "@/components/ui/button";
import { MenuItemCard } from "@/components/menuitem/menuItemCard";
import { toast } from "sonner";
import type { MenuItem } from "@/api/menuItemApi";

export default function MenuItemsPage() {
  const { user, hasPermission } = useAuth();
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();

  const [menuItems, setMenuItems] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState("");
  const [courseFilter, setCourseFilter] = useState("all");
  const [allergenFilter, setAllergenFilter] = useState("all");

  useEffect(() => {
    if (!user?.token) return;

    getMenuItems(user.token)
      .then(setMenuItems)
      .catch(() => toast.error("Failed to load menu items."))
      .finally(() => setLoading(false));
  }, [user]);

  const courses = [
    "all",
    ...new Set(menuItems.map((mi) => mi.course).filter(Boolean) as string[]),
  ];

  const allergens = [
    "all",
    ...new Set(
      menuItems.flatMap((mi) => mi.allergens.map((a) => a.allergenName)),
    ),
  ];

  const filtered = menuItems.filter((mi) => {
    const matchesSearch = mi.name.toLowerCase().includes(search.toLowerCase());
    const matchesCourse = courseFilter === "all" || mi.course === courseFilter;
    const matchesAllergen =
      allergenFilter === "all" ||
      mi.allergens.some((a) => a.allergenName === allergenFilter);
    return matchesSearch && matchesCourse && matchesAllergen;
  });

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading menu...</p>
      </div>
    );
  }

  return (
    <div>
      <PageHeader
        title="Menu Items"
        subtitle={`${menuItems.length} items${menuItems.length !== 1 ? "s" : ""}`}
        action={
          hasPermission("menuitem", "create") ? (
            <Button
              onClick={() => navigate(`/${slug}/menu-items/new`)}
              className="bg-primary text-primary-foreground"
            >
              New Menu Item
            </Button>
          ) : undefined
        }
      />

      <div className="flex flex-col sm:flex-row gap-3 mb-6">
        <input
          type="text"
          placeholder="Search menu items..."
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          className="w-full sm:w-64 px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring placeholder:text-muted-foreground"
        />
        <select
          value={courseFilter}
          onChange={(e) => setCourseFilter(e.target.value)}
          className="px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
        >
          {courses.map((c) => (
            <option key={c} value={c}>
              {c === "all" ? "All courses" : c}
            </option>
          ))}
        </select>
        <select
          value={allergenFilter}
          onChange={(e) => setAllergenFilter(e.target.value)}
          className="px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
        >
          {allergens.map((a) => (
            <option key={a} value={a}>
              {a === "all" ? "All allergens" : a}
            </option>
          ))}
        </select>
      </div>

      {filtered.length === 0 ? (
        <EmptyState
          title={
            search || courseFilter !== "all" || allergenFilter !== "all"
              ? "No menu items match your filters"
              : "No menu items yet"
          }
          description="Add your first menu item to get started."
          action={
            hasPermission("menuitem", "create") ? (
              <Button
                onClick={() => navigate(`/${slug}/menu-items/new`)}
                className="bg-primary text-primary-foreground"
              >
                New Menu Item
              </Button>
            ) : undefined
          }
        />
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {filtered.map((mi) => (
            <MenuItemCard key={mi.menuItemId} menuItem={mi} />
          ))}
        </div>
      )}
    </div>
  );
}
