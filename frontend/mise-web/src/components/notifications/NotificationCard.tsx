import { NotificationTypeBadge } from "./NotificationTypeBadge";
import type { NotificationItem } from "@/api/notificationApi";

interface NotificationCardProps {
  notification: NotificationItem;
  onMarkAsRead: (id: string) => void;
}

export function NotificationCard({
  notification: n,
  onMarkAsRead,
}: NotificationCardProps) {
  return (
    <div
      className={`p-4 rounded-lg border cursor-pointer transition-colors ${
        n.isRead
          ? "bg-card border-border opacity-60"
          : "bg-card border-border hover:border-primary"
      }`}
      onClick={() => !n.isRead && onMarkAsRead(n.notificationId)}
    >
      <div className="flex items-start gap-3">
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2 mb-1">
            <NotificationTypeBadge type={n.type} />
            {!n.isRead && (
              <span className="w-2 h-2 rounded-full bg-secondary flex-shrink-0" />
            )}
          </div>
          <p className="text-sm font-medium text-foreground">{n.title}</p>
          <p className="text-sm text-muted-foreground mt-0.5">{n.message}</p>
        </div>
        <span className="text-xs text-muted-foreground whitespace-nowrap flex-shrink-0">
          {new Date(n.createdAt).toLocaleDateString()}
        </span>
      </div>
    </div>
  );
}
