import { useState } from "react";
import { PageHeader } from "@/components/PageHeader";
import { CategoriesTab } from "@/components/system/CategoriesTab";
import { AllergensTab } from "@/components/system/AllergensTab";
import { UnitTypesTab } from "@/components/system/UnitTypesTab";
import { IngredientsTab } from "@/components/system/IngredientsTab";

type Tab = "categories" | "allergens" | "unitTypes" | "ingredients";

export default function SystemPage() {
  const [activeTab, setActiveTab] = useState<Tab>("ingredients");

  const tabs: { id: Tab; label: string }[] = [
    { id: "categories", label: "Categories" },
    { id: "allergens", label: "Allergens" },
    { id: "unitTypes", label: "Unit Types" },
    { id: "ingredients", label: "Ingredients" },
  ];

  return (
    <div>
      <PageHeader
        title="System"
        subtitle="Manage categories, allergens, unit types and ingredients"
      />

      <div className="flex gap-2 mb-6 border-b border-border">
        {tabs.map((tab) => (
          <button
            key={tab.id}
            onClick={() => setActiveTab(tab.id)}
            className={`text-sm px-4 py-2.5 border-b-2 transition-colors -mb-px ${
              activeTab === tab.id
                ? "border-primary text-primary font-medium"
                : "border-transparent text-muted-foreground hover:text-foreground"
            }`}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {activeTab === "categories" && <CategoriesTab />}
      {activeTab === "allergens" && <AllergensTab />}
      {activeTab === "unitTypes" && <UnitTypesTab />}
      {activeTab === "ingredients" && <IngredientsTab />}
    </div>
  );
}
