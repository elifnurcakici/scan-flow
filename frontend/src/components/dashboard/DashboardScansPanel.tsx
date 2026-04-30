import type { ScanListItem } from "@/types/api"

import { Badge } from "@/components/ui/badge"
import { ScrollArea } from "@/components/ui/scroll-area"
import { DashboardEntityHeader } from "./DashboardEntityCard"
import { DashboardPanelNotice } from "./DashboardPanelNotice"
import { DashboardPanelCard } from "./DashboardPanelCard"

export function DashboardScansPanel({
  scans,
  selectedScanId,
  isLoading,
  statusPillClass,
  onSelect,
}: {
  scans: ScanListItem[]
  selectedScanId: number | null
  isLoading: boolean
  statusPillClass: Record<string, "default" | "secondary" | "destructive" | "outline">
  onSelect: (scanId: number) => void
}) {
  return (
    <DashboardPanelCard
      badge="Pipeline"
      title="Live scans"
      description="Pick a scan to inspect details."
      contentClassName="min-h-0 flex-1"
      beam
    >
        <ScrollArea className="space-y-3">
        {isLoading ? (
          <DashboardPanelNotice message="Scans are loading..." compact />
        ) : scans.length === 0 ? (
          <DashboardPanelNotice message="Start a scan to populate this list." compact />
        ) : (
          scans.map((scan) => (
            <button
              key={scan.id}
              type="button"
              onClick={() => onSelect(scan.id)}
              className={[
                "group relative w-full rounded-[20px] border p-3.5 text-left transition",
                selectedScanId === scan.id
                  ? "border-emerald-400/30 bg-emerald-400/10 shadow-[0_0_30px_rgba(16,185,129,0.1)]"
                  : "border-white/8 bg-white/[0.025] hover:border-white/20 hover:bg-white/[0.04]",
              ].join(" ")}
            >
              <DashboardEntityHeader
                title={scan.name}
                subtitle={scan.assetName}
                action={<Badge variant={statusPillClass[scan.status] ?? "outline"}>{scan.status}</Badge>}
              />

              <div className="flex items-center justify-between text-[11px] text-slate-400">
                <span>{new Date(scan.createdAt).toLocaleString()}</span>
                <span>{scan.vulnerabilityCount} findings</span>
              </div>

              {scan.errorReason ? (
                <div className="mt-3 rounded-2xl border border-rose-400/16 bg-rose-400/10 px-3 py-2 text-xs text-rose-100">
                  {scan.errorReason}
                </div>
              ) : null}
            </button>
          ))
        )}
        </ScrollArea>
    </DashboardPanelCard>
  )
}
