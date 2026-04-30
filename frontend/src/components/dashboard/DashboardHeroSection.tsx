import { Activity, AlertTriangle, Radar } from "lucide-react"

import { Spotlight } from "@/components/effects/Spotlight"
import { Badge } from "@/components/ui/badge"
import { Card, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { DashboardMetricTile } from "./DashboardMetricTile"

export function DashboardHeroSection({
  assetCount,
  runningCount,
  criticalCount,
}: {
  assetCount: number
  runningCount: number
  criticalCount: number
}) {
  return (
    <Card className="relative overflow-hidden rounded-[26px] border-white/10 bg-white/[0.035]">
      <Spotlight className="right-10 top-0 bg-cyan-400/10" size="sm" />
      <div className="absolute inset-y-0 right-0 w-1/2 bg-[radial-gradient(circle_at_top,rgba(52,211,153,0.14),transparent_52%)]" />
      <CardHeader className="relative z-10 gap-3 pb-4">
        <div className="flex flex-col gap-3 xl:flex-row xl:items-center xl:justify-between">
          <div className="min-w-0">
            <Badge className="mb-3 w-fit">Live SecOps Console</Badge>
            <CardTitle className="max-w-2xl text-2xl font-semibold tracking-[-0.05em] text-white xl:text-[2.4rem]">
              See assets, scans and findings live.
            </CardTitle>
            <CardDescription className="mt-2 max-w-xl text-sm leading-6 text-slate-300/78">
              Start scans and follow status changes in one screen.
            </CardDescription>
          </div>

          <div className="grid gap-2 sm:grid-cols-3 xl:min-w-[360px]">
            <DashboardMetricTile
              icon={Radar}
              label="Assets"
              value={String(assetCount)}
              accent="emerald"
            />
            <DashboardMetricTile
              icon={Activity}
              label="Running"
              value={String(runningCount)}
              accent="cyan"
            />
            <DashboardMetricTile
              icon={AlertTriangle}
              label="Critical"
              value={String(criticalCount)}
              accent="rose"
            />
          </div>
        </div>
      </CardHeader>
    </Card>
  )
}
