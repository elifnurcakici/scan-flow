import { useEffect, useMemo } from "react"
import { useQuery } from "@tanstack/react-query"

import { http } from "@/lib/api"
import type { ApiResponse, Asset, ScanDetail, ScanListItem } from "@/types/api"

export function useDashboardData({
  selectedScanId,
  onSelectedScanChange,
}: {
  selectedScanId: number | null
  onSelectedScanChange: (scanId: number) => void
}) {
  const assetsQuery = useQuery({
    queryKey: ["assets"],
    queryFn: async () => {
      const response = await http.get<ApiResponse<Asset[]>>("/api/assets")
      return response.data.data ?? []
    },
  })

  const scansQuery = useQuery({
    queryKey: ["scans"],
    queryFn: async () => {
      const response = await http.get<ApiResponse<ScanListItem[]>>("/api/scans")
      return response.data.data ?? []
    },
  })

  const scanDetailQuery = useQuery({
    queryKey: ["scan-detail", selectedScanId],
    enabled: selectedScanId !== null,
    queryFn: async () => {
      const response = await http.get<ApiResponse<ScanDetail>>(`/api/scans/${selectedScanId}`)
      return response.data.data
    },
  })

  useEffect(() => {
    if (!scansQuery.data?.length) {
      return
    }

    if (selectedScanId === null) {
      onSelectedScanChange(scansQuery.data[0].id)
      return
    }

    const stillExists = scansQuery.data.some((scan) => scan.id === selectedScanId)
    if (!stillExists) {
      onSelectedScanChange(scansQuery.data[0].id)
    }
  }, [onSelectedScanChange, scansQuery.data, selectedScanId])

  const assets = assetsQuery.data ?? []
  const scans = scansQuery.data ?? []
  const selectedScan = scanDetailQuery.data ?? null

  const runningCount = useMemo(
    () => scans.filter((scan) => scan.status === "Running").length,
    [scans],
  )
  const finishedCount = useMemo(
    () => scans.filter((scan) => scan.status === "Finished").length,
    [scans],
  )
  const latestVulnerabilities = selectedScan?.vulnerabilities ?? []
  const criticalCount = useMemo(
    () =>
      latestVulnerabilities.filter((vulnerability) => vulnerability.severity === "Critical")
        .length,
    [latestVulnerabilities],
  )
  const recentFindings = useMemo(
    () => latestVulnerabilities.slice(0, 3),
    [latestVulnerabilities],
  )

  return {
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
  }
}
