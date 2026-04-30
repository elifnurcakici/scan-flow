import { useDeferredValue, useState } from "react"

import { DashboardAssetsPanel } from "@/components/dashboard/DashboardAssetsPanel"
import { DashboardHeroSection } from "@/components/dashboard/DashboardHeroSection"
import { DashboardScanDetailPanel } from "@/components/dashboard/DashboardScanDetailPanel"
import { DashboardScansPanel } from "@/components/dashboard/DashboardScansPanel"
import { DashboardSummaryPanel } from "@/components/dashboard/DashboardSummaryPanel"
import { statusPillClass } from "@/components/dashboard/dashboard-constants"
import { useDashboardActions } from "@/hooks/useDashboardActions"
import { useDashboardData } from "@/hooks/useDashboardData"

export default function DashboardPage() {
  const [selectedScanId, setSelectedScanId] = useState<number | null>(null)
  const deferredSelectedScanId = useDeferredValue(selectedScanId)

  const {
    assetForm,
    feedback,
    isSubmitting,
    setAssetForm,
    handleAssetSubmit,
    startScan,
    selectScan,
  } = useDashboardActions({
    onSelectedScanChange: setSelectedScanId,
  })

  const {
    assets,
    scans,
    selectedScan,
    runningCount,
    finishedCount,
    criticalCount,
    recentFindings,
    assetsQuery,
    scansQuery,
    scanDetailQuery,
  } = useDashboardData({
    selectedScanId: deferredSelectedScanId,
    onSelectedScanChange: setSelectedScanId,
  })

  return (
    <div className="flex h-full min-h-0 flex-col gap-4">
      <section className="grid gap-4 xl:grid-cols-[1.18fr_0.82fr]">
        <DashboardHeroSection
          assetCount={assets.length}
          runningCount={runningCount}
          criticalCount={criticalCount}
        />
        <DashboardSummaryPanel
          finishedCount={finishedCount}
          findings={recentFindings}
        />
      </section>

      <section className="grid min-h-0 flex-1 gap-4 xl:grid-cols-[0.92fr_1.02fr_1.06fr]">
        <DashboardAssetsPanel
          assetForm={assetForm}
          assets={assets}
          feedback={feedback}
          isLoading={assetsQuery.isLoading}
          isSubmitting={isSubmitting}
          onAssetFormChange={setAssetForm}
          onSubmit={handleAssetSubmit}
          onStartScan={startScan}
        />
        <DashboardScansPanel
          scans={scans}
          selectedScanId={selectedScanId}
          isLoading={scansQuery.isLoading}
          statusPillClass={statusPillClass}
          onSelect={selectScan}
        />
        <DashboardScanDetailPanel
          selectedScanId={selectedScanId}
          scan={selectedScan}
          isLoading={scanDetailQuery.isLoading}
          statusPillClass={statusPillClass}
        />
      </section>
    </div>
  )
}
