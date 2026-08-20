import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import {
  getPrepListById,
  completeItem,
  //forceCompleteItem,
  completePrepList,
  forceCompletePrepList,
  addPrepListItem,
  deletePrepListItem,
  assignPrepList,
} from "@/api/prepListApi";
import { getUsers } from "@/api/userApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import { AddPrepListItemForm } from "@/components/preplist/AddPrepListItemForm";
import { toast } from "sonner";
import type {
  AddPrepListItemRequest,
  PrepList,
  PrepListItem,
} from "@/api/prepListApi";
import type { UserItem } from "@/api/userApi";

export default function PrepListDetailPage() {
  const { user, hasPermission } = useAuth();
  const { slug, prepListId } = useParams<{
    slug: string;
    prepListId: string;
  }>();
  const navigate = useNavigate();

  const [prepList, setPrepList] = useState<PrepList | null>(null);
  const [loading, setLoading] = useState(true);
  const [users, setUsers] = useState<UserItem[]>([]);

  // authentication states
  const [isOwner, setIsOwner] = useState(false);
  const [canManage, setCanManage] = useState(false);
  const [canComplete, setCanComplete] = useState(false);

  // add item form state variables
  const [showAddItem, setShowAddItem] = useState(false);

  //assign prep list form
  const [showAssign, setShowAssign] = useState(false);
  const [assignUserId, setAssignUserId] = useState("");

  //complete list confirm dialog
  const [showCompleteConfirm, setShowCompleteConfirm] = useState(false);

  useEffect(() => {
    if (!user?.token || !prepListId) return;

    async function load() {
      try {
        const [prepData, userData] = await Promise.all([
          getPrepListById(user!.token, prepListId!),
          getUsers(user!.token),
        ]);
        console.log("PrepList data:", prepData);
        setPrepList(prepData);

        setUsers(userData);
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
      toast.error("Failed to complete item.");
    }
  }

  async function handleAddItem(request: AddPrepListItemRequest) {
    if (!user?.token || !prepListId) return;

    try {
      const updated = await addPrepListItem(user.token, prepListId, request);
      setPrepList(updated);
      setShowAddItem(false);
      toast.success("Item added.");
    } catch {
      toast.error("Failed to add item");
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
      navigate(`/${slug}/prep-lists`);
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
          <p className="text-sm font-medium text-foreground">Assign to user</p>
          <select
            value={assignUserId}
            onChange={(e) => setAssignUserId(e.target.value)}
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-background text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            <option value="">Select a user...</option>
            {users.map((u) => (
              <option key={u.userId} value={u.userId}>
                {u.firstName} {u.lastName} - {u.role}
              </option>
            ))}
          </select>
          <div className="flex gap-2">
            <Button
              onClick={handleAssign}
              disabled={!assignUserId}
              className="bg-primary text-primary-foreground"
            >
              Assign
            </Button>
            <Button
              variant="outline"
              onClick={() => {
                setShowAssign(false);
                setAssignUserId("");
              }}
            >
              Cancel
            </Button>
          </div>
        </div>
      )}

      <div className="space-y-2 mb-6">
        {prepList.items.map((item) => (
          <div
            key={item.prepListItemId}
            className={`flex items-center gap-3 p-4 rounded-lg border ${
              item.isComplete
                ? "bg-muted border-border opacity-60"
                : "bg-card border-border"
            }`}
          >
            {!prepList.isComplete && canComplete && !item.isComplete && (
              <input
                type="checkbox"
                checked={item.isComplete}
                onChange={() => handleCompleteItem(item)}
                className="w-5 h-5 rounded flex-shrink-0 cursor-pointer accent-secondary"
              />
            )}
            {(prepList.isComplete || !canComplete || item.isComplete) && (
              <div
                className={`w-5 h-5 rounded border flex items-center justify-center flex-shrink-0 ${
                  item.isComplete
                    ? "bg-secondary border-secondary"
                    : "border-border"
                }`}
              >
                {item.isComplete && (
                  <span className="text-white text-xs">check</span>
                )}
              </div>
            )}

            <div className="flex-1 min-w-0">
              <p
                className={`text-sm font-medium ${
                  item.isComplete
                    ? "line-through text-muted-foreground"
                    : "text-foreground"
                }`}
              >
                {item.itemName}
                {item.quantity && (
                  <span className="font-normal text-muted-foreground ml-2">
                    - {item.quantity} {item.unit ?? ""}
                  </span>
                )}
              </p>
              {item.notes && (
                <p className="text-xs text-muted-foreground mt-0.5">
                  {item.notes}
                </p>
              )}
              {item.completedByName && (
                <p className="text-xs text-muted-foreground mt-0.5">
                  Completed by {item.completedByName}
                </p>
              )}
            </div>

            {item.recipeId && item.recipeTitle && (
              <button
                onClick={() =>
                  navigate(
                    `/${slug}/recipes/${item.recipeId}/cook?returnTo=prep-list&prepListId=${prepListId}`,
                  )
                }
                className="text-xs text-secondary hover:underline flex-shrink-0"
              >
                Cook
              </button>
            )}

            {!prepList.isComplete && canManage && (
              <button
                onClick={() => handleDeleteItem(item.prepListItemId)}
                className="text-xs text-destructive hover:underline flex-shrink-0"
              >
                Remove
              </button>
            )}
          </div>
        ))}

        {!prepList.isComplete && hasPermission("preplist", "update") && (
          <div>
            {!showAddItem ? (
              <Button
                variant="outline"
                onClick={() => setShowAddItem(true)}
                className="w-full"
              >
                + Add Item
              </Button>
            ) : (
              <AddPrepListItemForm
                currentItemCount={prepList.items.length}
                onItemAdded={handleAddItem}
                onCancel={() => setShowAddItem(false)}
              />
            )}
          </div>
        )}
      </div>
    </div>
  );
}
