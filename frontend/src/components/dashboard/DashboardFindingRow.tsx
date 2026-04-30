import { ArrowUpRight } from "lucide-react"

import type { VulnerabilityItem } from "@/types/api"
import { Badge } from "@/components/ui/badge"

export function DashboardFindingRow({
  finding,
  detailed = false,
}: {
  finding: VulnerabilityItem
  detailed?: boolean
}) {
  return (
    <div className="rounded-[18px] border border-white/8 bg-white/[0.025] p-3">
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <div className="flex items-center gap-2 text-sm font-medium text-white">
            <span className="size-2 rounded-full bg-emerald-300 shadow-[0_0_12px_rgba(110,231,183,0.85)]" />
            <span className="truncate">{finding.type}</span>
          </div>
          {detailed ? (
            <p className="mt-2 text-xs leading-5 text-slate-400">{finding.description}</p>
          ) : null}
        </div>
        <Badge variant={severityVariant(finding.severity)}>{finding.severity}</Badge>
      </div>
      <div className="mt-2 flex items-center justify-between text-[11px] text-slate-500">
        <span>{finding.createdAt ? new Date(finding.createdAt).toLocaleString() : "Live feed"}</span>
        <ArrowUpRight className="size-3.5" />
      </div>
    </div>
  )
}

function severityVariant(severity: string) {
  if (severity === "Critical" || severity === "High") {
    return "destructive" as const
  }

  if (severity === "Medium") {
    return "warning" as const
  }

  return "outline" as const
}
