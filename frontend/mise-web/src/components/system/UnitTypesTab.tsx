import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import {
  getUnitTypes,
  createUnitType,
  deleteUnitType,
} from "@/api/unitTypeApi";
import { Button } from "../ui/button";
import { toast } from "sonner";
import { inputClass, selectClass } from "@/lib/styles";
import type { UnitTypeItem } from "@/api/unitTypeApi";

export function UnitTypesTab() {
  const { user, hasPermission } = useAuth();
  const [unitTypes, setUnitTypes] = useState<UnitTypeItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [newName, setNewName] = useState("");
  const [newAbbreviation, setNewAbbreviation] = useState("");
  const [newMeasureType, setNewMeasureType] = useState("volume");
  const [newSystem, setNewSystem] = useState("metric");
  const [creating, setCreating] = useState(false);

  useEffect(() => {
    if (!user?.token) return;
    getUnitTypes(user.token)
      .then(setUnitTypes)
      .catch(() => toast.error("Failed to load unit types."))
      .finally(() => setLoading(false));
  }, [user]);

  async function handleCreate() {
    if (!user?.token || !newName.trim() || !newAbbreviation.trim()) return;
    setCreating(true);
    try {
      const created = await createUnitType(user.token, {
        name: newName,
        abbreviation: newAbbreviation,
        measureType: newMeasureType as any,
        system: newSystem as any,
      });
      setUnitTypes((prev) => [...prev, created]);
      setNewName("");
      setNewAbbreviation("");
      setNewMeasureType("volume");
      setNewSystem("metric");
      setShowForm(false);
      toast.success("Unit type created.");
    } catch {
      toast.error("Failed to create unit type.");
    } finally {
      setCreating(false);
    }
  }

  async function handleDelete(unitTypeId: string) {
    if (!user?.token) return;
    try {
      await deleteUnitType(user.token, unitTypeId);
      setUnitTypes((prev) => prev.filter((u) => u.unitTypeId !== unitTypeId));
      toast.success("Unit type deleted.");
    } catch {
      toast.error("Failed to delete unit type. It may be in use.");
    }
  }

  if (loading)
    return <p className="text-sm text-muted-foreground">Loading...</p>;

  const grouped = unitTypes.reduce(
    (acc, u) => {
      const key = `${u.system} - ${u.measureType}`;
      if (!acc[key]) acc[key] = [];
      acc[key].push(u);
      return acc;
    },
    {} as Record<string, UnitTypeItem[]>,
  );

  return (
    <div className="max-w-lg space-y-6">
      {Object.entries(grouped)
        .sort()
        .map(([group, units]) => (
          <div key={group}>
            <h3 className="text-sm font-medium text-muted-foreground uppercase tracking-wide mb-3">
              {group}
            </h3>
            <div className="space-y-2">
              {units.map((u) => (
                <div
                  key={u.unitTypeId}
                  className="flex items-center gap-3 p-3 bg-card rounded-lg border border-border"
                >
                  <span className="flex-1 text-sm text-foreground">
                    {u.name}
                  </span>
                  <span className="text-xs text-muted-foreground">
                    {u.abbreviation}
                  </span>
                  {hasPermission("unit", "delete") && (
                    <button
                      onClick={() => handleDelete(u.unitTypeId)}
                      className="text-xs text-destructive hover:underline"
                    >
                      Delete
                    </button>
                  )}
                </div>
              ))}
            </div>
          </div>
        ))}

      {hasPermission("unit", "create") &&
        (!showForm ? (
          <Button variant="outline" onClick={() => setShowForm(true)}>
            + Add unit type
          </Button>
        ) : (
          <div className="p-4 bg-card rounded-lg border border-border space-y-3">
            <div className="flex gap-2">
              <input
                type="text"
                value={newName}
                onChange={(e) => setNewName(e.target.value)}
                placeholder="Name (e.g. Kilogram)"
                className={inputClass}
                autoFocus
              />
              <input
                type="text"
                value={newAbbreviation}
                onChange={(e) => setNewAbbreviation(e.target.value)}
                placeholder="Abbr (e.g. kg)"
                className="w-28 px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
              />
            </div>
            <div className="flex gap-2">
              <select
                value={newMeasureType}
                onChange={(e) => setNewMeasureType(e.target.value)}
                className={selectClass}
              >
                <option value="volume">Volume</option>
                <option value="weight">Weight</option>
                <option value="count">Count</option>
                <option value="length">Length</option>
                <option value="temperature">Temperature</option>
              </select>
              <select
                value={newSystem}
                onChange={(e) => setNewSystem(e.target.value)}
                className={selectClass}
              >
                <option value="metric">Metric</option>
                <option value="imperial">Imperial</option>
                <option value="universal">Universal</option>
              </select>
            </div>
            <div className="flex gap-2">
              <Button
                onClick={handleCreate}
                disabled={
                  creating || !newName.trim() || !newAbbreviation.trim()
                }
                className="bg-primary text-primary-foreground"
              >
                {creating ? "Adding..." : "Add"}
              </Button>
              <Button variant="outline" onClick={() => setShowForm(false)}>
                Cancel
              </Button>
            </div>
          </div>
        ))}
    </div>
  );
}
