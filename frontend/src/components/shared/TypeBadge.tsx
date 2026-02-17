import { typeLabels } from "@/lib/utils";

interface TypeBadgeProps {
  type: string;
}

export function TypeBadge({ type }: TypeBadgeProps) {
  const isExport = type === "Export";
  const color = isExport
    ? "bg-emerald-100 text-emerald-800"
    : "bg-sky-100 text-sky-800";
  const label = typeLabels[type] || type;

  return (
    <span className={`inline-flex items-center rounded-full text-sm font-medium px-2.5 py-1 ${color}`}>
      {isExport ? "↑" : "↓"} {label}
    </span>
  );
}
