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
  flagItemIncomplete,
  unflagItemIncomplete,
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
import axios from "axios";
import { ReasonPicker } from "@/components/preplist/ReasonPicker";
import {
  EMPTY_REASON,
  getReasonLabel,
  isReasonComplete,
} from "@/config/incompleteReasons";
import type { IncompleteReasonValue } from "@/config/incompleteReasons";

function errorMessage(err: unknown, fallback: string): string {
  if (axios.isAxiosError(err) && err.response?.data?.errors?.[0]) {
    return err.response.data.errors[0];
  }
  return fallback;
}

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

  const [canManage, setCanManage] = useState(false);
  const [canCheckItems, setCanCheckItems] = useState(false);
  const [canCompleteList, setCanCompleteList] = useState(false);

  // add item form state variables
  const [showAddItem, setShowAddItem] = useState(false);

  //assign prep list form
  const [showAssign, setShowAssign] = useState(false);
  const [assignUserId, setAssignUserId] = useState("");

  //complete list confirm dialog
  const [showCompleteConfirm, setShowCompleteConfirm] = useState(false);
  const [flaggedItemId, setFlaggingItemId] = useState<string | null>(null);
  const [flagReason, setFlagReason] =
    useState<IncompleteReasonValue>(EMPTY_REASON);

  // reasons for untouched items
  const [untouchedReasons, setUntouchedReasons] = useState<
    Record<string, IncompleteReasonValue>
  >({});

  useEffect(() => {
    if (!user?.token || !prepListId) return;

    async function load() {
      try {
        const manage = hasPermission("preplist", "manage");

        const [prepData, userData] = await Promise.all([
          getPrepListById(user!.token, prepListId!),
          manage ? getUsers(user!.token) : Promise.resolve([]),
        ]);
        setPrepList(prepData);

        setUsers(userData);
        const owner = prepData.createdBy === user!.userId;
        const assignee = prepData.assignedTo === user!.userId;
        const canUpdate = hasPermission("preplist", "update");
        const canCompletePermission = hasPermission("preplist", "complete");

        setCanManage(manage);
        setCanCheckItems(owner || manage || (assignee && canUpdate));
        setCanCompleteList(
          owner || manage || (assignee && canCompletePermission),
        );
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

    if (!canCheckItems) {
      toast.error("You can only complete items on your own prep lists.");
      return;
    }

    try {
      const updated = await completeItem(
        user.token,
        prepListId,
        item.prepListItemId,
      );
      setPrepList(updated);
      toast.success(`${item.itemName} marked as complete.`);
    } catch {
      toast.error("Failed to complete item.");
    }
  }

  async function handleFlagItem(item: PrepListItem) {
    if (!user?.token || !prepListId) return;

    try {
      const updated = await flagItemIncomplete(
        user.token,
        prepListId,
        item.prepListItemId,
        flagReason.reasonCode,
        flagReason.reasonNote.trim() || null,
      );
      setPrepList(updated);
      setFlaggingItemId(null);
      setFlagReason(EMPTY_REASON);
      toast.success(`${item.itemName} marked incomplete.`);
    } catch (err) {
      toast.error(errorMessage(err, "Failed to mark item incomplete."));
    }
  }

  async function handleUnflagItem(item: PrepListItem) {
    if (!user?.token || !prepListId) return;

    try {
      const updated = await unflagItemIncomplete(
        user.token,
        prepListId,
        item.prepListItemId,
      );
      setPrepList(updated);
      toast.success(`${item.itemName} is back on the list.`);
    } catch (err) {
      toast.error(errorMessage(err, "Failed to clear the flag."));
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
    if (!user?.token || !prepListId || !prepList) return;

    if (!canCompleteList) {
      toast.error("You can only complete your own prep lists.");
      return;
    }

    const untouched = prepList.items.filter(
      (i) => !i.isComplete && !i.isIncomplete,
    );

    try {
      const updated = await completePrepList(user.token, prepListId, {
        untouchedItemReasons: untouched.map((i) => {
          const reason = untouchedReasons[i.prepListItemId] ?? EMPTY_REASON;
          return {
            prepListItemId: i.prepListItemId,
            reasonCode: reason.reasonCode,
            reasonNote: reason.reasonNote.trim() || null,
          };
        }),
      });
      setPrepList(updated);
      setShowCompleteConfirm(false);
      toast.success("Prep list completed.");
      navigate(`/${slug}/prep-lists`);
    } catch (err) {
      toast.error(errorMessage(err, "Failed to complete prep list."));
    }
  }

  async function handleForceCompletePrepList() {
    if (!user?.token || !prepListId || !canManage) return;

    try {
      const updated = await forceCompletePrepList(user.token, prepListId);
      setPrepList(updated);
      setShowCompleteConfirm(false);
      toast.success("Prep list completed.");
      navigate(`/${slug}/prep-lists`);
    } catch (err) {
      toast.error(errorMessage(err, "Failed to complete prep list."));
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

  const flaggedItems = prepList.items.filter(
    (i) => i.isIncomplete && i.isComplete,
  ).length;
  const untouchedItems = prepList.items.filter(
    (i) => !i.isComplete && !i.isIncomplete,
  );
  const allReasonsGiven = untouchedItems.every((i) =>
    isReasonComplete(untouchedReasons[i.prepListItemId] ?? EMPTY_REASON),
  );

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
                {prepList.assignedTo ? "Reassign" : "Assign"}
              </Button>
            )}
            {canCompleteList && !prepList.isComplete && (
              <Button
                onClick={() => setShowCompleteConfirm(true)}
                className="bg-primary text-primary-foreground"
              >
                Complete list
              </Button>
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
        <span className="text-sm text-muted-foreground whitespace-nowrap">
          {completedItems} / {totalItems} items
          {flaggedItems > 0 && ` ${flaggedItems} incompleted`}
        </span>
      </div>

      {prepList.isComplete && (
        <div className="mb-4 px-4 py-3 rounded-lg bg-green-50 border border-green-200 text-green-800 text-sm font-medium">
          Prep list complete
        </div>
      )}

      {showCompleteConfirm && !prepList.isComplete && (
        <div className="mb-4 p-4 bg-card rounded-lg border border-border space-y-4">
          <p className="text-sm font-medium text-foreground">
            {untouchedItems.length === 0
              ? "Mark this prep list complete?"
              : `${untouchedItems.length} item${untouchedItems.length !== 1 ? "s" : ""} not done. Give a reason for each so the team knows what happened.`}
          </p>
          {untouchedItems.map((item) => (
            <div key={item.prepListItemId} className="space-y-2">
              <p className="text-sm text-foreground">{item.itemName}</p>
              <ReasonPicker
                value={untouchedReasons[item.prepListItemId] ?? EMPTY_REASON}
                onChange={(v) =>
                  setUntouchedReasons((prev) => ({
                    ...prev,
                    [item.prepListItemId]: v,
                  }))
                }
              />
            </div>
          ))}
          <div className="flex flex-wrap gap-2">
            <Button
              onClick={handleCompletePrepList}
              disabled={!allReasonsGiven}
              className="bg-primary text-primary-foreground"
            >
              Complete list
            </Button>
            <Button
              variant="outline"
              onClick={() => setShowCompleteConfirm(false)}
            >
              Cancel
            </Button>
            {canManage && untouchedItems.length > 0 && (
              <Button variant="outline" onClick={handleForceCompletePrepList}>
                Mark all done instead
              </Button>
            )}
          </div>
        </div>
      )}

      {showAssign && (
        <div className="mb-4 p-4 bg-card rounded-lg border border-border space-y-3">
          <p className="text-sm font-medium text-foreground">Assign to user</p>
          <select
            value={assignUserId}
            onChange={(e) => setAssignUserId(e.target.value)}
            className="w-full px-4 py-2.5 rounded-lg border border-border bg-card text-foreground text-sm focus:outline-none focus:ring-2 focus:ring-ring"
          >
            <option value="">Select a user...</option>
            {users.map((u) => (
              <option key={u.userId} value={u.userId}>
                {u.firstName} {u.lastName}
                {u.roles.length > 0 ? ` - ${u.roles.join(", ")}` : ""}
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
            {!prepList.isComplete && canCheckItems && !item.isComplete && (
              <input
                type="checkbox"
                checked={item.isComplete}
                onChange={() => handleCompleteItem(item)}
                className="w-5 h-5 rounded flex-shrink-0 cursor-pointer accent-secondary"
              />
            )}
            {(prepList.isComplete || !canCheckItems || item.isComplete) && (
              <div
                className={`w-5 h-5 rounded border flex items-center justify-center flex-shrink-0 ${
                  item.isComplete
                    ? "bg-secondary border-secondary"
                    : "border-border"
                }`}
              >
                {item.isComplete && (
                  <span className="text-white text-xs">✓</span>
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
