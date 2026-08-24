import { Button } from "../ui/button";
import type { UserItem } from "@/api/userApi";

interface UserCardProps {
  user: UserItem;
  currentUserId: string;
  canManage: boolean;
  onDeactivate: (userId: string) => void;
  onReactivate: (userId: string) => void;
}

export function UserCard({
  user: u,
  currentUserId,
  canManage,
  onDeactivate,
  onReactivate,
}: UserCardProps) {
  const isCurrentUser = u.userId === currentUserId;

  return (
    <div className="flex items-center gap-4 p-4 bg-card rounded-lg border border-border">
      <div className="w-10 h-10 rounded-full bg-primary flex items-center justify-center text-primary-foreground text-sm font-medium flex-shrink-0">
        {u.firstName[0]}
        {u.lastName[0]}
      </div>
      <div className="flex-1 min-w-0">
        <div className="flex items-center gap-2">
          <p className="text-sm font-medium text-foreground">
            {u.firstName} {u.lastName}
            {isCurrentUser && (
              <span className="text-xs text-muted-foreground ml-1">(you)</span>
            )}
          </p>
          <span
            className={`text-xs px-2 py-0.5 rounded-full font-medium ${
              u.status === "active"
                ? "bg-green-100 text-green-800"
                : u.status === "pending"
                  ? "bg-yellow-100 text-yellow-800"
                  : "bg-red-100 text-red-800"
            }`}
          >
            {u.status}
          </span>
        </div>
        <p className="text-xs text-muted-foreground">{u.email}</p>
        {u.roles.length > 0 && (
          <p className="text-xs text-muted-foreground mt-0.5">
            {u.roles.join(", ")}
          </p>
        )}
      </div>
      {!isCurrentUser && canManage && (
        <div className="flex-shrink-0">
          {u.status === "active" || u.status === "pending" ? (
            <Button
              variant="outline"
              onClick={() => onDeactivate(u.userId)}
              className="text-xs h-8 px-3 text-destructive border-destructive"
            >
              Deactivate
            </Button>
          ) : (
            <Button
              variant="outline"
              onClick={() => onReactivate(u.userId)}
              className="text-xs h-8 px-3"
            >
              Reactivate
            </Button>
          )}
        </div>
      )}
    </div>
  );
}
