function getLabel(type: string): string {
  switch (type) {
    case "recipe_published":
      return "New recipe";
    case "recipe_updated":
      return "Recipe updated";
    case "preplist_assigned":
      return "Prep list assigned";
    case "direct_message":
      return "Message";
    case "system_message":
      return "System";
    default:
      return type;
  }
}

function getColor(type: string): string {
  switch (type) {
    case "recipe_published":
      return "bg-green-100 text-green-800";
    case "recipe_updated":
      return "bg-blue-100 text-blue-100";
    case "preplist_assigned":
      return "bg-purple-100 text-purple-800";
    case "direct_message":
      return "bg-secondary/20 text-secondary";
    case "system_message":
      return "bg-red-100 text-red-800";
    default:
      return "bg-muted text-muted-foreground";
  }
}

interface NotificationTypeBadgeProps {
  type: string;
}

export function NotificationTypeBadge({ type }: NotificationTypeBadgeProps) {
  return (
    <span
      className={`text-xs px-2 py-0.5 rounded-full font-medium ${getColor(type)}`}
    >
      {getLabel(type)}
    </span>
  );
}
