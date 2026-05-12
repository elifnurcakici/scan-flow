import { useQuery, useQueryClient } from "@tanstack/react-query"

import { PageHeader } from "@/components/workspace/PageHeader"
import { StatusBadge } from "@/components/workspace/StatusBadge"
import { Card, CardContent } from "@/components/ui/card"
import { http } from "@/lib/api"
import { useScanSocket } from "@/sockets/useScanSocket"
import type { ApiResponse, ScanListItem } from "@/types/api"

function formatDate(value: string) {
  return new Date(value).toLocaleString("tr-TR")
}

export default function ScansPage() {
  const queryClient = useQueryClient()

  useScanSocket({
    onScanUpdated: ({ scanId }) => {
      void queryClient.invalidateQueries({ queryKey: ["scans"] })
      void queryClient.invalidateQueries({ queryKey: ["scan-detail", scanId] })
      void queryClient.invalidateQueries({ queryKey: ["vulnerabilities"] })
      void queryClient.invalidateQueries({ queryKey: ["assets"] })
    },
  })

  const scansQuery = useQuery({
    queryKey: ["scans"],
    queryFn: async () => {
      const response = await http.get<ApiResponse<ScanListItem[]>>("/api/scans")
      return response.data.data ?? []
    },
  })

  const scans = scansQuery.data ?? []

  return (
    <div className="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
      <PageHeader
        eyebrow="Scans"
        title="Scan activity"
        description="Watch scan lifecycle changes in real time. This page stays connected to the socket and refreshes when scanners publish updates."
      />

      <Card className="flex min-h-0 flex-1 flex-col border-slate-800 bg-slate-950/80 text-white">
        <CardContent className="min-h-0 flex-1 overflow-auto p-0">
          <table className="min-w-full text-sm">
            <thead className="bg-slate-900/80 text-left text-slate-400">
              <tr>
                <th className="px-4 py-3">Scan</th>
                <th className="px-4 py-3">Asset</th>
                <th className="px-4 py-3">Scanner</th>
                <th className="px-4 py-3">Status</th>
                <th className="px-4 py-3">Findings</th>
                <th className="px-4 py-3">Critical</th>
                <th className="px-4 py-3">High</th>
                <th className="px-4 py-3">Started at</th>
              </tr>
            </thead>
            <tbody>
              {scans.map((scan) => (
                <tr key={scan.id} className="border-t border-slate-800">
                  <td className="px-4 py-3">{scan.name}</td>
                  <td className="px-4 py-3 text-slate-300">{scan.assetName}</td>
                  <td className="px-4 py-3 text-slate-300">
                    {scan.scannerName} <span className="text-slate-500">({scan.scannerType})</span>
                  </td>
                  <td className="px-4 py-3"><StatusBadge status={scan.status} /></td>
                  <td className="px-4 py-3">{scan.vulnerabilityCount}</td>
                  <td className="px-4 py-3">{scan.criticalCount}</td>
                  <td className="px-4 py-3">{scan.highCount}</td>
                  <td className="px-4 py-3 text-slate-300">{formatDate(scan.createdAt)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </CardContent>
      </Card>
    </div>
  )
}
