interface MenuItemStatusBadgeProps {
  status: string;
}

export function MenuItemStatusBadge({ status }: MenuItemStatusBadgeProps) {
  return (
    <span
      className={`text-xs px-2 py-0.5 rounded-full font-medium ${
        status === "published"
          ? "bg-green-100 text-green-800"
          : "bg-yellow-100 text-yellow-800"
      }`}
    >
      {status}
    </span>
  );
}
