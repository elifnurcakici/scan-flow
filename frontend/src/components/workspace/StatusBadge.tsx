import { Badge } from "@/components/ui/badge"
import { cn } from "@/lib/utils"

const statusClasses: Record<string, string> = {
  Pending: "border-amber-400/30 bg-amber-400/10 text-amber-200",
  Running: "border-sky-400/30 bg-sky-400/10 text-sky-200",
  Finished: "border-emerald-400/30 bg-emerald-400/10 text-emerald-200",
  Failed: "border-rose-400/30 bg-rose-400/10 text-rose-200",
}

export function StatusBadge({ status }: { status: string }) {
  return (
    <Badge className={cn("border px-2.5 py-1 text-xs font-medium", statusClasses[status])}>
      {status}
    </Badge>
  )
}
