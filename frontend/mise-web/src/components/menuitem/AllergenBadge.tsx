interface AllergenBadgeProps {
  name: string;
  isMajor: boolean;
  isManual?: boolean;
}

export function AllergenBadge({ name, isMajor, isManual }: AllergenBadgeProps) {
  return (
    <span
      className={`text-xs px-2 py-0.5 rounded-full ${
        isMajor ? "bg-red-100 text-red-800" : "bg-orange-100 text-orange-800"
      }`}
    >
      {name}
      {isManual && " *"}
    </span>
  );
}
