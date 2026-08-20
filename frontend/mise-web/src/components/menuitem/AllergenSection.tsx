import { useNavigate, useParams } from "react-router-dom";
import { AllergenBadge } from "./AllergenBadge";
import { Button } from "../ui/button";
import type { MenuItemAllergen } from "@/api/menuItemApi";

interface AllergenSectionProps {
  allergens: MenuItemAllergen[];
  canManage: boolean;
  menuItemId: string;
  resolving: boolean;
  onResolve: () => void;
  onRemove: (allergenid: string) => void;
}

export function AllergenSection({
  allergens,
  canManage,
  menuItemId,
  resolving,
  onResolve,
  onRemove,
}: AllergenSectionProps) {
  const { slug } = useParams<{ slug: string }>();
  const navigate = useNavigate();

  const majorAllergens = allergens.filter((a) => a.isMajor);
  const minorAllregens = allergens.filter((a) => !a.isMajor);

  return (
    <section className="mb=6">
      <div className="flex items-center justify-between mb-4 pb-2 border-b border-border">
        <h2 className="text-lg font-semibold text-foreground">Allergens</h2>
        {canManage && (
          <div className="flex gap-2">
            <Button
              variant="outline"
              onClick={onResolve}
              disabled={resolving}
              className="text-xs h-8 px-3"
            >
              {resolving ? "Resolving..." : "REsolving allergens"}
            </Button>
            <Button
              variant="outline"
              onClick={() =>
                navigate(`/${slug}/menu-items/${menuItemId}/allergens`)
              }
              className="text-xs h-8 px-3"
            >
              Add manual
            </Button>
          </div>
        )}
      </div>

      {allergens.length === 0 ? (
        <p className="text-sm text-muted-foreground">No allergens tagged.</p>
      ) : (
        <div className="space-y-4">
          {majorAllergens.length > 0 && (
            <div>
              <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-2">
                Major allergens
              </p>
              <div className="flex flex-wrap gap-2">
                {majorAllergens.map((a) => (
                  <div
                    key={a.menuItemAllergenId}
                    className="flex items-center gap-1"
                  >
                    <AllergenBadge
                      name={a.allergenName}
                      isMajor={a.isMajor}
                      isManual={a.isManual}
                    />
                    {canManage && a.isManual && (
                      <button
                        onClick={() => onRemove(a.menuItemAllergenId)}
                        className="text-xs text-destructive hover:underline"
                      >
                        X
                      </button>
                    )}
                  </div>
                ))}
              </div>
            </div>
          )}
          {minorAllregens.length > 0 && (
            <div>
              <p className="text-xs font-medium text-muted-foreground uppercase tracking-wide mb-2">
                Other allergens
              </p>
              <div className="flex flex-wrap gap-2">
                {minorAllregens.map((a) => (
                  <div
                    key={a.menuItemAllergenId}
                    className="flex items-center gap-1"
                  >
                    <AllergenBadge
                      name={a.allergenName}
                      isMajor={a.isMajor}
                      isManual={a.isManual}
                    />
                    {canManage && a.isManual && (
                      <button
                        onClick={() => onRemove(a.menuItemAllergenId)}
                        className="text-xs text-destructive hover:underline"
                      >
                        X
                      </button>
                    )}
                  </div>
                ))}
              </div>
            </div>
          )}
          <p className="text-xs text-muted-foreground">
            * manually added allergen
          </p>
        </div>
      )}
    </section>
  );
}
