import { Gauge, Sparkles } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import type { VulnerabilityItem } from "@/types/api"
import { detectionSeries } from "./dashboard-constants"
import { DashboardFindingRow } from "./DashboardFindingRow"
import { DashboardEntityCard } from "./DashboardEntityCard"
import { DashboardPanelNotice } from "./DashboardPanelNotice"
import { DashboardPanelCard } from "./DashboardPanelCard"
import { DashboardSummaryStat } from "./DashboardSummaryStat"

export function DashboardSummaryPanel({
  finishedCount,
  findings,
}: {
  finishedCount: number
  findings: VulnerabilityItem[]
}) {
  return (
    <DashboardPanelCard
      badge="Summary"
      title="Risk snapshot"
      description="Recent activity and findings at a glance."
      contentClassName="space-y-4"
      action={<Badge variant="outline">Live</Badge>}
      beam
      cardClassName="bg-[#07111a]/82"
    >
        <div className="grid gap-3 sm:grid-cols-2">
          <DashboardSummaryStat icon={Gauge} label="Finished" value={String(finishedCount)} />
          <DashboardSummaryStat icon={Sparkles} label="Findings" value={String(findings.length)} />
        </div>

        <DashboardEntityCard className="bg-white/[0.02] p-3">
          <div className="mb-3 flex items-center justify-between">
            <h3 className="text-xs font-medium uppercase tracking-[0.18em] text-slate-300">
              Detection energy
            </h3>
            <span className="text-xs text-slate-500">pulse</span>
          </div>

          <div className="flex h-20 items-end gap-1.5">
            {detectionSeries.map((value, index) => (
              <div key={index} className="relative flex-1 rounded-full bg-white/[0.04]">
                <div
                  className="absolute inset-x-0 bottom-0 rounded-full bg-gradient-to-t from-emerald-500 to-emerald-300 shadow-[0_0_20px_rgba(16,185,129,0.22)]"
                  style={{ height: `${value}%` }}
                />
              </div>
            ))}
          </div>
        </DashboardEntityCard>

        <div className="space-y-2">
          {findings.length === 0 ? (
            <DashboardPanelNotice message="Start a scan to see findings." compact />
          ) : (
            findings.map((finding) => <DashboardFindingRow key={finding.id} finding={finding} />)
          )}
        </div>
    </DashboardPanelCard>
  )
}
