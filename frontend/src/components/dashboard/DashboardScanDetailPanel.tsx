import { Globe } from "lucide-react"

import type { ScanDetail } from "@/types/api"
import { Badge } from "@/components/ui/badge"
import { ScrollArea } from "@/components/ui/scroll-area"
import { DashboardEntityCard } from "./DashboardEntityCard"
import { DashboardFindingRow } from "./DashboardFindingRow"
import { DashboardPanelNotice } from "./DashboardPanelNotice"
import { DashboardPanelCard } from "./DashboardPanelCard"

export function DashboardScanDetailPanel({
  selectedScanId,
  scan,
  isLoading,
  statusPillClass,
}: {
  selectedScanId: number | null
  scan: ScanDetail | null
  isLoading: boolean
  statusPillClass: Record<string, "default" | "secondary" | "destructive" | "outline">
}) {
  return (
    <DashboardPanelCard
      badge="Inspection"
      title="Scan detail"
      description="Timeline and vulnerabilities for the selected run."
      contentClassName="min-h-0 flex-1"
      beam
    >
        <ScrollArea className="space-y-4">
        {selectedScanId === null ? (
          <DashboardPanelNotice message="Select a scan to inspect it." compact />
        ) : isLoading ? (
          <DashboardPanelNotice message="Loading detail..." compact />
        ) : scan ? (
          <>
            <DashboardEntityCard className="bg-white/[0.03] p-4">
              <div className="mb-4 flex items-start justify-between gap-4">
                <div className="min-w-0">
                  <div className="truncate text-base font-medium text-white">{scan.name}</div>
                  <div className="mt-1 flex items-center gap-2 truncate text-xs text-slate-400">
                    <Globe className="size-3.5 shrink-0" />
                    {scan.assetName} · {scan.assetDomain}
                  </div>
                </div>
                <Badge variant={statusPillClass[scan.status] ?? "outline"}>
                  {scan.status}
                </Badge>
              </div>

              {scan.errorReason ? (
                <div className="rounded-2xl border border-rose-400/16 bg-rose-400/10 px-3 py-2 text-sm text-rose-100">
                  {scan.errorReason}
                </div>
              ) : null}

              <div className="mt-4 space-y-2.5">
                {scan.history.map((historyItem, index) => (
                  <div key={historyItem.id} className="flex items-start gap-3">
                    <div className="flex flex-col items-center">
                      <span className="mt-1 size-2.5 rounded-full bg-emerald-300 shadow-[0_0_14px_rgba(110,231,183,0.9)]" />
                      {index < scan.history.length - 1 ? (
                        <span className="mt-2 h-8 w-px bg-white/10" />
                      ) : null}
                    </div>
                    <div className="rounded-[18px] border border-white/8 bg-white/[0.025] px-3 py-2.5">
                      <div className="text-sm font-medium text-white">{historyItem.status}</div>
                      <div className="mt-1 text-[11px] text-slate-400">
                        {new Date(historyItem.createdAt).toLocaleString()}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </DashboardEntityCard>

            <div className="space-y-2.5">
              <div className="flex items-center justify-between">
                <h3 className="text-xs font-medium uppercase tracking-[0.18em] text-slate-300">
                  Vulnerabilities
                </h3>
                <span className="text-[11px] text-slate-500">
                  {scan.vulnerabilities.length} records
                </span>
              </div>

              {scan.vulnerabilities.length === 0 ? (
                <DashboardPanelNotice message="No vulnerabilities recorded." compact />
              ) : (
                scan.vulnerabilities.map((vulnerability) => (
                  <DashboardFindingRow key={vulnerability.id} finding={vulnerability} detailed />
                ))
              )}
            </div>
          </>
        ) : (
          <DashboardPanelNotice message="This scan could not be loaded." compact />
        )}
        </ScrollArea>
    </DashboardPanelCard>
  )
}
