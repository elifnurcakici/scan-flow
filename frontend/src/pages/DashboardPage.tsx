import { useMemo } from "react"
import { useQuery } from "@tanstack/react-query"

import { PageHeader } from "@/components/workspace/PageHeader"
import { StatusBadge } from "@/components/workspace/StatusBadge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { http } from "@/lib/api"
import type { ApiResponse, Asset, ScanListItem, VulnerabilityListItem } from "@/types/api"

function formatDate(value: string) {
  return new Date(value).toLocaleString("tr-TR", {
    day: "2-digit",
    month: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  })
}

export default function DashboardPage() {
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

  const vulnerabilitiesQuery = useQuery({
    queryKey: ["vulnerabilities"],
    queryFn: async () => {
      const response =
        await http.get<ApiResponse<VulnerabilityListItem[]>>("/api/vulnerabilities")
      return response.data.data ?? []
    },
  })

  const assets = assetsQuery.data ?? []
  const scans = scansQuery.data ?? []
  const vulnerabilities = vulnerabilitiesQuery.data ?? []

  const summary = useMemo(() => {
    return {
      assetCount: assets.length,
      runningCount: scans.filter((scan) => scan.status === "Running").length,
      criticalCount: vulnerabilities.filter((item) => item.severity === "Critical").length,
      findingCount: vulnerabilities.length,
    }
  }, [assets, scans, vulnerabilities])

  return (
    <div className="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
      <PageHeader
        eyebrow="Overview"
        title="Workspace dashboard"
        description="See how many assets you own, which scans are active and where critical findings are accumulating."
      />

      <section className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        {[
          { label: "Assets", value: summary.assetCount },
          { label: "Running scans", value: summary.runningCount },
          { label: "Critical findings", value: summary.criticalCount },
          { label: "Total findings", value: summary.findingCount },
        ].map((item) => (
          <Card key={item.label} className="border-slate-800 bg-slate-950/80 text-white">
            <CardHeader className="pb-2">
              <CardTitle className="text-sm font-medium text-slate-400">{item.label}</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="text-3xl font-semibold">{item.value}</div>
            </CardContent>
          </Card>
        ))}
      </section>

      <section className="grid min-h-0 flex-1 gap-4 xl:grid-cols-2">
        <Card className="flex min-h-0 flex-col border-slate-800 bg-slate-950/80 text-white">
          <CardHeader>
            <CardTitle>Latest scans</CardTitle>
          </CardHeader>
          <CardContent className="min-h-0 flex-1 space-y-3 overflow-y-auto">
            {scans.slice(0, 5).map((scan) => (
              <div
                key={scan.id}
                className="flex flex-col gap-2 rounded-2xl border border-slate-800 bg-slate-900/60 p-4"
              >
                <div className="flex items-center justify-between gap-3">
                  <div>
                    <div className="font-medium">{scan.name}</div>
                    <div className="text-sm text-slate-400">
                      {scan.assetName} • {scan.scannerName}
                    </div>
                  </div>
                  <StatusBadge status={scan.status} />
                </div>
                <div className="text-xs text-slate-500">{formatDate(scan.createdAt)}</div>
              </div>
            ))}
          </CardContent>
        </Card>

        <Card className="flex min-h-0 flex-col border-slate-800 bg-slate-950/80 text-white">
          <CardHeader>
            <CardTitle>Recent vulnerabilities</CardTitle>
          </CardHeader>
          <CardContent className="min-h-0 flex-1 space-y-3 overflow-y-auto">
            {vulnerabilities.slice(0, 5).map((item) => (
              <div
                key={item.id}
                className="rounded-2xl border border-slate-800 bg-slate-900/60 p-4"
              >
                <div className="flex items-center justify-between gap-3">
                  <div className="font-medium">{item.type}</div>
                  <StatusBadge status={item.severity === "Critical" ? "Failed" : "Finished"} />
                </div>
                <div className="mt-1 text-sm text-slate-400">
                  {item.assetName} • {item.cweId ?? "No CWE"}
                </div>
                <div className="mt-2 text-sm text-slate-300">{item.description}</div>
              </div>
            ))}
          </CardContent>
        </Card>
      </section>
    </div>
  )
}
