import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { useNotifications } from "@/context/NotificationContext";
import {
  getUnreadNotifications,
  markAsRead,
  markAllAsRead,
} from "@/api/notificationApi";
import { PageHeader } from "@/components/PageHeader";
import { Button } from "@/components/ui/button";
import { NotificationCard } from "@/components/notifications/NotificationCard";
import { toast } from "sonner";
import type { NotificationItem } from "@/api/notificationApi";

export default function NotificationsPage() {
  const { user } = useAuth();
  const { clearUnread, decrementUnread } = useNotifications();

  const [notifications, setNotifications] = useState<NotificationItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<"all" | "unread">("unread");

  useEffect(() => {
    if (!user?.token) return;

    getUnreadNotifications(user.token)
      .then(setNotifications)
      .catch(() => toast.error("Failed to load notifications."))
      .finally(() => setLoading(false));
  }, [user]);

  async function handleMarkAsRead(notificationId: string) {
    if (!user?.token) return;
    try {
      await markAsRead(user.token, notificationId);
      setNotifications((prev) =>
        prev.map((n) =>
          n.notificationId === notificationId ? { ...n, isRead: true } : n,
        ),
      );
      decrementUnread();
    } catch {
      toast.error("Failed to mark as read.");
    }
  }

  async function handleMarkAllAsRead() {
    if (!user?.token) return;
    try {
      await markAllAsRead(user.token);
      setNotifications((prev) => prev.map((n) => ({ ...n, isRead: true })));
      clearUnread();
      toast.success("All notification marked as read.");
    } catch {
      toast.error("Failed to mark all as read.");
    }
  }

  const filtered =
    filter === "unread"
      ? notifications.filter((n) => !n.isRead)
      : notifications;

  const unreadCount = notifications.filter((n) => !n.isRead).length;

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading notifications...</p>
      </div>
    );
  }

  return (
    <div className="max-w-2xl">
      <PageHeader
        title="Notifications"
        subtitle={unreadCount > 0 ? `${unreadCount} unread` : "All caught up"}
        action={
          unreadCount > 0 ? (
            <Button variant="outline" onCanPlay={handleMarkAllAsRead}>
              Mark all as read
            </Button>
          ) : undefined
        }
      />

      <div className="flex gap-2 mb-6">
        {(["unread", "all"] as const).map((f) => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={`text-sm px-4 py-2 rounded-lg transition-colors ${
              filter === f
                ? "bg-primary text-primary-foreground border-primary"
                : "bg-card text-foreground border-border hover:border-primary"
            }`}
          >
            {f === "unread" ? `Unread (${unreadCount})` : "All"}
          </button>
        ))}
      </div>
      {filtered.length === 0 ? (
        <div className="text-center py-16">
          <p className="text-muted-foreground">
            {filter === "unread"
              ? "No unread notifications"
              : "No notifications yet."}
          </p>
        </div>
      ) : (
        <div className="space-y-2">
          {filtered.map((n) => (
            <NotificationCard
              key={n.notificationId}
              notification={n}
              onMarkAsRead={handleMarkAsRead}
            />
          ))}
        </div>
      )}
    </div>
  );
}
