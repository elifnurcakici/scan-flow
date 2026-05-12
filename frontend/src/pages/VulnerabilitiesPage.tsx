import { useQuery, useQueryClient } from "@tanstack/react-query"

import { PageHeader } from "@/components/workspace/PageHeader"
import { Card, CardContent } from "@/components/ui/card"
import { http } from "@/lib/api"
import { useScanSocket } from "@/sockets/useScanSocket"
import type { ApiResponse, VulnerabilityListItem } from "@/types/api"

function formatCvss(item: VulnerabilityListItem) {
  if (item.cvssScore === null) {
    return "N/A"
  }

  return `${item.cvssScore.toFixed(1)}${item.cvssVector ? ` • ${item.cvssVector}` : ""}`
}

export default function VulnerabilitiesPage() {
  const queryClient = useQueryClient()

  useScanSocket({
    onScanUpdated: () => {
      void queryClient.invalidateQueries({ queryKey: ["vulnerabilities"] })
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

  const vulnerabilities = vulnerabilitiesQuery.data ?? []

  return (
    <div className="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
      <PageHeader
        eyebrow="Findings"
        title="Vulnerability inventory"
        description="Browse findings with asset, scan, severity, CWE and CVSS context."
      />

      <Card className="flex min-h-0 flex-1 flex-col border-slate-800 bg-slate-950/80 text-white">
        <CardContent className="min-h-0 flex-1 overflow-auto p-0">
          <table className="min-w-full text-sm">
            <thead className="bg-slate-900/80 text-left text-slate-400">
              <tr>
                <th className="px-4 py-3">Type</th>
                <th className="px-4 py-3">Severity</th>
                <th className="px-4 py-3">Asset</th>
                <th className="px-4 py-3">Scan</th>
                <th className="px-4 py-3">CWE</th>
                <th className="px-4 py-3">CVSS</th>
              </tr>
            </thead>
            <tbody>
              {vulnerabilities.map((item) => (
                <tr key={item.id} className="border-t border-slate-800">
                  <td className="px-4 py-3">
                    <div className="font-medium">{item.type}</div>
                    <div className="mt-1 text-xs text-slate-400">{item.description}</div>
                  </td>
                  <td className="px-4 py-3">{item.severity}</td>
                  <td className="px-4 py-3 text-slate-300">
                    {item.assetName}
                    <div className="text-xs text-slate-500">{item.assetDomain}</div>
                  </td>
                  <td className="px-4 py-3 text-slate-300">{item.scanName}</td>
                  <td className="px-4 py-3">{item.cweId ?? "N/A"}</td>
                  <td className="px-4 py-3 text-slate-300">{formatCvss(item)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </CardContent>
      </Card>
    </div>
  )
}
