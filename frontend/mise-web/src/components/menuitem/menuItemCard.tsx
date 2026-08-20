import { useNavigate, useParams } from "react-router-dom";
import { AllergenBadge } from "./AllergenBadge";
import type { MenuItem } from "@/api/menuItemApi";

interface MenuItemCardProps {
  menuItem: MenuItem;
}

export function MenuItemCard({ menuItem }: MenuItemCardProps) {
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();

  return (
    <div
      onClick={() => navigate(`/${slug}/menu-items/${menuItem.menuItemId}`)}
      className="bg-card rounded-lg p-4 border border-border cursor-pointer hover:border-primary transition-colors"
    >
      <div className="flex items-start justify-between gap-2 mb-2">
        <h3 className="font-medium text-foreground text-sm">{menuItem.name}</h3>
        <span
          className={`text-xs px-2 py-0.5 rounded-full font-medium flex-shrink-0 ${
            menuItem.status === "published"
              ? "bg-green-100 text-green-800"
              : "bg-yellow-100 text-yellow-800"
          }`}
        >
          {menuItem.status}
        </span>
      </div>
      {menuItem.course && (
        <p className="text-xs text-muted-foreground mb-2">{menuItem.course}</p>
      )}
      {menuItem.allergens.length > 0 ? (
        <div className="flex flex-wrap gap-1 mt-2">
          {menuItem.allergens.slice(0, 4).map((a) => (
            <AllergenBadge
              key={a.menuItemAllergenId}
              name={a.allergenName}
              isMajor={a.isMajor}
              isManual={a.isManual}
            />
          ))}
          {menuItem.allergens.length > 4 && (
            <span className="text-xs text-muted-foreground">
              +{menuItem.allergens.length - 4} more
            </span>
          )}
        </div>
      ) : (
        <p className="text-xs text-muted-foreground mt-2">
          No allergens tagged.
        </p>
      )}
    </div>
  );
}
