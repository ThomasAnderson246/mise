import { Link, useLocation, useParams } from "react-router-dom";
import { useAuth } from "@/context/AuthContext";
import { cn } from "@/lib/utils";

interface NavItem {
  label: string;
  path: string;
  icon: string;
  permission?: { resource: string; action: string };
}

const navItems: NavItem[] = [
  { label: "Dashboard", path: "dashboard", icon: "" },
  {
    label: "Recipes",
    path: "recipes",
    icon: "",
    permission: { resource: "recipe", action: "read" },
  },
  {
    label: "Prep Lists",
    path: "prep-lists",
    icon: "",
    permission: { resource: "preplist", action: "read" },
  },
  {
    label: "Ingredients",
    path: "ingredients",
    icon: "",
    permission: { resource: "ingredient", action: "read" },
  },
  {
    label: "Menu Items",
    path: "menu-items",
    icon: "",
    permission: { resource: "menuitem", action: "read" },
  },
  {
    label: "Categories",
    path: "categories",
    icon: "",
    permission: { resource: "category", action: "read" },
  },
  {
    label: "Allergens",
    path: "allergens",
    icon: "",
    permission: { resource: "allergen", action: "read" },
  },
  {
    label: "Unit Types",
    path: "units",
    icon: "",
    permission: { resource: "unit", action: "read" },
  },
  {
    label: "Users",
    path: "users",
    icon: "",
    permission: { resource: "user", action: "manage" },
  },
  {
    label: "Roles",
    path: "roles",
    icon: "",
    permission: { resource: "user", action: "manage" },
  },
  {
    label: "Audit Log",
    path: "audit",
    icon: "",
    permission: { resource: "audit", action: "read" },
  },
  {
    label: "Notifications",
    path: "notifications",
    icon: "",
    permission: { resource: "notification", action: "read" },
  },
];

export function Sidebar() {
  const { slug } = useParams<{ slug: string }>();
  const { hasPermission, user, logout } = useAuth();
  const location = useLocation();

  const visibleItems = navItems.filter(
    (item) =>
      !item.permission ||
      hasPermission(item.permission.resource, item.permission.action),
  );

  function handleLogout() {
    logout();
  }

  return (
    <aside className="md:flex flex-col w-56 min-h-screen bg-sidebar border-r border-sidebar-border">
      <div className="px-4 py-5 border-b border-sidebar-border">
        <span className="text-xl font-bold text-secondary">Mise</span>
      </div>

      <nav className="flex-1 px-2 py-4 space-y-1 overflow-y-auto">
        {visibleItems.map((item) => {
          const fullPath = `/${slug}/${item.path}`;
          const isActive = location.pathname == fullPath;

          return (
            <Link
              key={item.path}
              to={fullPath}
              className={cn(
                "flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm transition-colors",
                isActive
                  ? "bg-primary text-primary-foreground font-medium"
                  : "text-sidebar-foreground hover:bg-muted",
              )}
            >
              <span className="text-base">{item.icon}</span>
              {item.label}
            </Link>
          );
        })}
      </nav>
      <div className="px-4 py-4 border-t border-sidebar-border">
        <div className="flex items-center gap-3 mb-3">
          <div className="w-8 h-8 rounded-full bg-primary flex items-center justify-center text-primary-foreground text-xs font-medium">
            {user?.firstName?.[0]}
            {user?.lastName?.[0]}
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-sm font-medium text-sidebar-foreground truncate">
              {user?.firstName} {user?.lastName}
            </p>
          </div>
          <p className="text-xs text-muted-foreground truncate">{user?.role}</p>
        </div>
        <button
          onClick={handleLogout}
          className="w-full text-left text-sm text-muted-foreground hover:text-foreground transition-colors px-1"
        >
          Sign Out
        </button>
      </div>
    </aside>
  );
}
