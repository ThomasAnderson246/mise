import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { PageHeader } from "@/components/PageHeader";
import { getPrepListSummary } from "@/api/prepListApi";
import { getUnreadNotifications } from "@/api/notificationApi";
import { getRecipes } from "@/api/recipeApi";
import type { PrepListSummary } from "@/api/prepListApi";
import type { NotificationItem } from "@/api/notificationApi";
import { toast } from "sonner";

export default function DashboardPage() {
  const { user } = useAuth();

  const [prepLists, setPrepLists] = useState<PrepListSummary[]>([]);
  const [notifications, setNotifications] = useState<NotificationItem[]>([]);
  const [recipeCount, setRecipeCount] = useState(0);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!user?.token) return;

    async function loadDashboard() {
      try {
        const [prepData, notifData, recipeData] = await Promise.all([
          getPrepListSummary(user!.token),
          getUnreadNotifications(user!.token),
          getRecipes(user!.token),
        ]);
        setPrepLists(prepData);
        setNotifications(notifData);
        setRecipeCount(recipeData.length);

        if (notifData.length > 0) {
          toast.info(
            `You have ${notifData.length} unread notification${notifData.length !== 1 ? "s" : ""}`,
          );
        }
      } catch (err) {
        console.error("Dashboard load error:", err);
      } finally {
        setLoading(false);
      }
    }

    loadDashboard();
  }, [user]);

  function getGreeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return "Good morning";
    if (hour < 17) return "Good afternoon";
    return "Good evening";
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <p className="text-muted-foreground">Loading...</p>
      </div>
    );
  }

  return (
    <div>
      <PageHeader
        title={`${getGreeting()}, ${user?.firstName}`}
        subtitle={new Date().toLocaleDateString(`en-US`, {
          weekday: "long",
          month: "long",
          day: "numeric",
        })}
      />

      <div className="grid grid-cols-2 md:grid-cols-3 gap-4 mb-8">
        <div className="bg-card rounded-lg p-4 border border-border">
          <p className="text-sm text-muted-foreground mb-1">
            Active prep lists
          </p>
          <p className="text-2xl font-semibold text-secondary">
            {prepLists.length}
          </p>
        </div>
        <div className="bg-card rounded-lg p-4 border border-border">
          <p className="text-sm text-muted-foreground mb-1">Recipes</p>
          <p className="text-2xl font-semibold text-secondary">{recipeCount}</p>
        </div>
        <div className="bg-card rounded-lg p-4 border border-border col-span-2 md:col-span-1">
          <p className="text-sm text-muted-foreground mb-1">
            Unread notifications
          </p>
          <p className="text-2xl font-semibold text-secondary">
            {notifications.length}
          </p>
        </div>
      </div>

      <div className="mb-8">
        <h2 className="text-lg font-semibold text-foreground mb-3">
          Active Preplists
        </h2>
        {prepLists.length === 0 ? (
          <div className="bg-card rounded-lg p-6 border border-border text-center">
            <p className="text-muted-foreground text-sm">
              No active prep lists.
            </p>
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-3">
            {prepLists.map((pl) => (
              <div
                key={pl.prepListId}
                className="bg-card rounded-lg p-4 border border-border"
              >
                {pl.createdByName && (
                  <p className="text-xs text-muted-foreground mb-3">
                    {pl.createdByName}
                  </p>
                )}
                <div className="flex items-center justify-between">
                  <div className="flex-1 bg-muted rounded-full h-1.6 mr-3">
                    <div
                      className="bg-secondary h-1.5 rounded-full transition-all"
                      style={{
                        width:
                          pl.totalItems > 0
                            ? `${(pl.completedItems / pl.totalItems) * 100}%`
                            : "0%",
                      }}
                    />
                  </div>
                  <span className="text-xs font-medium text-secondary whitespace-nowrap">
                    {pl.completedItems} / {pl.totalItems}
                  </span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <div>
        <h2 className="text-lg font-semibold text-foreground mb-3">
          Unread notifications
        </h2>
        {notifications.length === 0 ? (
          <div className="bg-card rounded-lg p-6 border border-border text-center">
            <p className="text-muted-foreground text-sm">
              You're all caught up.
            </p>
          </div>
        ) : (
          <div className="space-y-2">
            {notifications.slice(0, 5).map((n) => (
              <div
                key={n.notificationId}
                className="bg-card rounded-lg p-4 border border-border"
              >
                <div className="flex items-start jusitfy-between gap-3">
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-foreground">
                      {n.title}
                    </p>
                    <p className="text-sm text-muted-foreground mt-0.5 truncate">
                      {n.message}
                    </p>
                  </div>
                  <span className="text-xs text-muted-foreground whitespace-nowrap">
                    {new Date(n.createdAt).toLocaleDateString()}
                  </span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
