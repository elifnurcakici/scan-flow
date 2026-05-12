import { useState } from "react"
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query"

import { PageHeader } from "@/components/workspace/PageHeader"
import { Button } from "@/components/ui/button"
import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Select } from "@/components/ui/select"
import { http } from "@/lib/api"
import type { ApiResponse, Asset, AssetType } from "@/types/api"

const assetTypeOptions: { label: string; value: AssetType }[] = [
  { label: "Domain", value: 1 },
  { label: "IP", value: 2 },
  { label: "Web App", value: 3 },
  { label: "Repository", value: 4 },
]

export default function AssetsPage() {
  const queryClient = useQueryClient()
  const [name, setName] = useState("")
  const [domain, setDomain] = useState("")
  const [type, setType] = useState<AssetType>(1)

  const assetsQuery = useQuery({
    queryKey: ["assets"],
    queryFn: async () => {
      const response = await http.get<ApiResponse<Asset[]>>("/api/assets")
      return response.data.data ?? []
    },
  })

  const createAssetMutation = useMutation({
    mutationFn: async () => {
      await http.post("/api/assets", { name, domain, type })
    },
    onSuccess: async () => {
      setName("")
      setDomain("")
      setType(1)
      await queryClient.invalidateQueries({ queryKey: ["assets"] })
    },
  })

  const startScanMutation = useMutation({
    mutationFn: async (asset: Asset) => {
      await http.post("/api/scans/start", {
        name: `${asset.name} Baseline Scan`,
        assetId: asset.id,
      })
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["scans"] })
      await queryClient.invalidateQueries({ queryKey: ["assets"] })
    },
  })

  const assets = assetsQuery.data ?? []

  return (
    <div className="flex h-full min-h-0 flex-col gap-4 overflow-hidden">
      <PageHeader
        eyebrow="Assets"
        title="Asset inventory"
        description="Track what you own, how many times it was scanned and where critical or high findings are accumulating."
        action={
          <form
            className="grid gap-2 md:grid-cols-[1fr_1fr_180px_120px]"
            onSubmit={(event) => {
              event.preventDefault()
              void createAssetMutation.mutateAsync()
            }}
          >
            <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="Asset name" className="border-slate-800 bg-slate-900 text-white" />
            <Input value={domain} onChange={(e) => setDomain(e.target.value)} placeholder="example.com / repo url" className="border-slate-800 bg-slate-900 text-white" />
            <Select
              value={String(type)}
              onChange={(event) => setType(Number(event.target.value) as AssetType)}
              className="border-slate-800 bg-slate-900 text-white"
            >
              {assetTypeOptions.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </Select>
            <Button type="submit" className="rounded-2xl bg-emerald-400 text-slate-950 hover:bg-emerald-300">
              Add asset
            </Button>
          </form>
        }
      />

      <Card className="flex min-h-0 flex-1 flex-col border-slate-800 bg-slate-950/80 text-white">
        <CardContent className="min-h-0 flex-1 overflow-auto p-0">
          <table className="min-w-full text-sm">
            <thead className="bg-slate-900/80 text-left text-slate-400">
              <tr>
                <th className="px-4 py-3">Name</th>
                <th className="px-4 py-3">Domain</th>
                <th className="px-4 py-3">Type</th>
                <th className="px-4 py-3">Scans</th>
                <th className="px-4 py-3">Findings</th>
                <th className="px-4 py-3">Critical</th>
                <th className="px-4 py-3">High</th>
                <th className="px-4 py-3">Action</th>
              </tr>
            </thead>
            <tbody>
              {assets.map((asset) => (
                <tr key={asset.id} className="border-t border-slate-800">
                  <td className="px-4 py-3">{asset.name}</td>
                  <td className="px-4 py-3 text-slate-300">{asset.domain}</td>
                  <td className="px-4 py-3 text-slate-300">{assetTypeOptions.find((item) => item.value === asset.type)?.label}</td>
                  <td className="px-4 py-3">{asset.scanCount}</td>
                  <td className="px-4 py-3">{asset.vulnerabilityCount}</td>
                  <td className="px-4 py-3">{asset.criticalCount}</td>
                  <td className="px-4 py-3">{asset.highCount}</td>
                  <td className="px-4 py-3">
                    <Button
                      type="button"
                      variant="outline"
                      className="rounded-2xl border-slate-700 bg-slate-900 text-white hover:bg-slate-800"
                      onClick={() => void startScanMutation.mutateAsync(asset)}
                    >
                      Start scan
                    </Button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </CardContent>
      </Card>
    </div>
  )
}
