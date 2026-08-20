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

  return <div className="max-w-2xl"></div>;
}
