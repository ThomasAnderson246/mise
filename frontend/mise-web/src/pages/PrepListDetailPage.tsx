import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import {
  getPrepListById,
  completeItem,
  forceCompleteItem,
  completePrepList,
  forceCompletePrepList,
  addPrepListItem,
  deletePrepListItem,
  assignPrepList,
} from "@/api/prepListApi";
import { getRecipes } from "@/api/recipeApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import { toast } from "sonner";
import type { PrepList, PrepListItem } from "@/api/prepListApi";
import type { RecipeItem } from "@/api/recipeApi";

export default function PrepListDetailPage() {
  const { user, hasPermission } = useAuth();
  const { slug, prepListId } = useParams<{
    slug: string;
    prepListId: string;
  }>();
  const navigate = useNavigate();

  const [prepList, setPrepList] = useState<PrepList | null>(null);
  const [loading, setLoading] = useState(true);
  const [recipes, setRecipes] = useState<RecipeItem[]>([]);

  // authentication states
  const [isOwner, setIsOwner] = useState(false);
  const [canManage, setCanManage] = useState(false);
  const [canComplete, setCanComplete] = useState(false);

  // add item form state variables
  const [showAddItem, setShowAddItem] = useState(false);
  const [itemName, setItemName] = useState("");
  const [itemQuantity, setItemQuantity] = useState("");
  const [itemUnit, setItemUnit] = useState("");
  const [itemRecipeId, setItemRecipeId] = useState("");
  const [itemNotes, setItemNotes] = useState("");
  const [addingItem, setAddingItem] = useState(false);

  //assign prep list form
  const [showAssign, setShowAssign] = useState(false);
  const [assignUserId, setAssignUserId] = useState("");

  //complete list confirm dialog
  const [showCompleteConfirm, setShowCompleteConfirm] = useState(false);

  useEffect(() => {
    if (!user?.token || !prepListId) return;

    async function load() {
      try {
        const [prepData, recipeData] = await Promise.all([
          getPrepListById(user!.token, prepListId!),
          getRecipes(user!.token),
        ]);
        setPrepList(prepData);
        setRecipes(recipeData);
        const owner = prepData.createdBy === user!.userId;
        const manage = hasPermission("preplist", "manage");
        setIsOwner(owner);
        setCanManage(manage);
        setCanComplete(owner || manage);
      } catch {
        toast.error("Failed to load prep list.");
        navigate(`/${slug}/prep-lists`);
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [user, prepListId]);

  async function handleCompleteItem(item: PrepListItem) {
    if (!user?.token || !prepListId) return;

    try {
      let updated: PrepList;
      if (canManage || isOwner) {
        updated = await completeItem(
          user.token,
          prepListId,
          item.prepListItemId,
        );
      } else {
        toast.error("You can only complete items on your own prep lists.");
        return;
      }
      setPrepList(updated);
      toast.success(`${item.itemName} marked as complete.`);
    } catch {
      toast.error("Failed to omplete item.");
    }
  }

  async function handleAddItem() {
    if (!user?.token || !prepListId || itemName.trim()) return;

    setAddingItem(true);

    try {
      const updated = await addPrepListItem(user.token, prepListId, {
        itemName,
        quantity: itemQuantity ? parseFloat(itemQuantity) : null,
        unit: itemUnit || null,
        recipeId: itemRecipeId || null,
        notes: itemNotes || null,
      });
      setPrepList(updated);
      setItemName("");
      setItemQuantity("");
      setItemUnit("");
      setItemRecipeId("");
      setItemNotes("");
      setShowAddItem(false);
      toast.success("Item added.");
    } catch {
      toast.error("Failed to add item.");
    } finally {
      setAddingItem(false);
    }
  }

  async function handleDeleteItem(itemId: string) {
    if (!user?.token || !prepListId) return;

    try {
      const updated = await deletePrepListItem(user.token, prepListId, itemId);
      setPrepList(updated);
      toast.success("Item removed.");
    } catch {
      toast.error("Failed to remove item.");
    }
  }

  async function handleCompletePrepList() {
    if (!user?.token || !prepListId) return;

    try {
      let updated: PrepList;
      if (canManage) {
        updated = await forceCompletePrepList(user.token, prepListId);
      } else if (isOwner) {
        updated = await completePrepList(user.token, prepListId);
      } else {
        toast.error("You can only complete your own prep lists.");
        return;
      }
      setPrepList(updated);
      setShowCompleteConfirm(false);
      toast.success("Prep list completed.");
    } catch {
      toast.error(
        "failed to complete prep list. Make sure all items are checked off.",
      );
    }
  }

  async function handleAssign() {
    if (!user?.token || !prepListId || !assignUserId) return;
    try {
      const updated = await assignPrepList(
        user.token,
        prepListId,
        assignUserId,
      );
      setPrepList(updated);
      setShowAssign(false);
      setAssignUserId("");
      toast.success("Prep list assigned.");
    } catch {
      toast.error("Failed to assign prep list.");
    }
  }
  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading...</p>
      </div>
    );
  }

  if (!prepList) return null;

  const totalItems = prepList.items.length;
  const completedItems = prepList.items.filter((i) => i.isComplete).length;
  const progress = totalItems > 0 ? (completedItems / totalItems) * 100 : 0;

  return (
    <div className="max-w-2xl">
      <PageHeader
        title={prepList.name}
        subtitle={
          prepList.assignedToName
            ? `Assigned to ${prepList.assignedToName}`
            : undefined
        }
        action={
          <div className="flex gap-2">
            {canManage && !prepList.isComplete && (
              <Button variant="outline" onClick={() => setShowAssign(true)}>
                Assign
              </Button>
            )}
            {canComplete && !prepList.isComplete && (
              <>
                {showCompleteConfirm ? (
                  <div className="flex items-center gap-2">
                    <span className="text-sm text-muted-foreground">
                      Mark complete?
                    </span>
                    <Button
                      onClick={handleCompletePrepList}
                      className="bg-primary text-primary-foreground"
                    >
                      Confirm
                    </Button>
                    <Button
                      variant="outline"
                      onClick={() => setShowCompleteConfirm(false)}
                    >
                      Cancel
                    </Button>
                  </div>
                ) : (
                  <Button
                    onClick={() => setShowCompleteConfirm(true)}
                    className="bg-primary text-primary-foreground"
                  >
                    Complete list
                  </Button>
                )}
              </>
            )}
          </div>
        }
      />

      <div className="flex items-center gap-3 mb-6">
        <div className="flex-1 bg-muted rounded-full h-2">
          <div
            className="bg-secondary h-2 rounded-full transition-all"
            style={{ width: `${progress}%` }}
          />
        </div>
        <span className="text-sm text-muted-foreground whitespace-wrap">
          {completedItems} / {totalItems} items
        </span>
      </div>

      {prepList.isComplete && (
        <div className="mb-4 px-4 py-3 rounded-lg bg-green-50 border border-green-200 text-green-800 text-sm font-medium">
          Prep list complete
        </div>
      )}

      {showAssign && (
        <div className="mb-4 p-4 bg-card rounded-lg border border-border space-y-3">
          <p className="text-sm font-medium text-foreground">Assign to cook</p>
          <input
            type="text"
            value={assignUserId}
            onChange={(e) => setAssignUserId(e.target.value)}
            placeholder="User id..."
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          />
          <div className="flex gap-2">
            <Button
              onClick={handleAssign}
              className="bg-primary text-primary-foreground"
            >
              Assign
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}
