import { useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { createIngredient } from "@/api/ingredientApi";
import { Button } from "../ui/button";
import { toast } from "sonner";
import { inputClass, selectClass } from "@/lib/styles";
import axios from "axios";
import type { IngredientItem } from "@/api/ingredientApi";
import type { UnitTypeItem } from "@/api/unitTypeApi";

interface NewIngredientFormProps {
  unitTypes: UnitTypeItem[];
  onCreated: (ingredient: IngredientItem) => void;
  onCancel: () => void;
  initialName?: string;
}

export function NewIngredientForm({
  unitTypes,
  onCreated,
  onCancel,
  initialName = "",
}: NewIngredientFormProps) {
  const { user } = useAuth();
  const [name, setName] = useState(initialName);
  const [category, setCategory] = useState("");
  const [unitTypeId, setUnitTypeId] = useState("");
  const [isNonConvertible, setIsNonConvertible] = useState(false);
  const [saving, setSaving] = useState(false);

  async function handleCreate() {
    if (!user?.token || !name.trim()) return;
    setSaving(true);

    try {
      const created = await createIngredient(user.token, {
        name,
        category: category || null,
        defaultUnitTypeId: unitTypeId || null,
        isNonConvertible,
        allergenIds: [],
      });
      onCreated(created);
      toast.info(
        `${created.name} created. Remember to tag allergens in ingredient management.`,
      );
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data?.errors?.[0]) {
        toast.error(err.response.data.errors[0]);
      } else {
        toast.error("Failed to create ingredient");
      }
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="bg-card rounded-lg p-4 border border-border space-y-3">
      <p className="text-sm font-medium text-foreground">
        Create new ingredient
      </p>
      <input
        type="text"
        value={name}
        onChange={(e) => setName(e.target.value)}
        placeholder="Ingredient name"
        className={inputClass}
        autoFocus
      />
      <input
        type="text"
        value={category}
        onChange={(e) => setCategory(e.target.value)}
        placeholder="Category (optional)"
        className={inputClass}
      />
      <select
        value={unitTypeId}
        onChange={(e) => setUnitTypeId(e.target.value)}
        className={selectClass}
      >
        <option value="">No default unit</option>
        {unitTypes.map((ut) => (
          <option key={ut.unitTypeId} value={ut.unitTypeId}>
            {ut.name} ({ut.abbreviation})
          </option>
        ))}
      </select>
      <label className="flex items-center gap-2 text-sm text-foreground">
        <input
          type="checkbox"
          checked={isNonConvertible}
          onChange={(e) => setIsNonConvertible(e.target.checked)}
        />
        Non-convertible unit
      </label>
      <div className="flex gap-2">
        <Button
          onClick={handleCreate}
          disabled={saving || !name.trim()}
          className="bg-primary text-primary-foreground"
        >
          {saving ? "Creating..." : "Create ingredient"}
        </Button>
        <Button variant="outline" onClick={onCancel}>
          Cancel
        </Button>
      </div>
    </div>
  );
}
