import type { Asset, AssetType } from "@/types/api"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Select } from "@/components/ui/select"
import { Separator } from "@/components/ui/separator"
import { TerminalSquare } from "lucide-react"
import { DashboardEntityCard, DashboardEntityHeader } from "./DashboardEntityCard"
import { DashboardPanelNotice } from "./DashboardPanelNotice"
import { DashboardPanelCard, DashboardPanelSpotlight } from "./DashboardPanelCard"
import { assetTypeLabels } from "./dashboard-constants"

type CreateAssetRequest = {
  name: string
  domain: string
  type: AssetType
}

export function DashboardAssetsPanel({
  assetForm,
  assets,
  feedback,
  isLoading,
  isSubmitting,
  onAssetFormChange,
  onSubmit,
  onStartScan,
}: {
  assetForm: CreateAssetRequest
  assets: Asset[]
  feedback: string | null
  isLoading: boolean
  isSubmitting: boolean
  onAssetFormChange: (nextValue: CreateAssetRequest) => void
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void
  onStartScan: (asset: Asset) => void
}) {
  return (
    <DashboardPanelCard
      badge="Assets"
      title="Register a target"
      description="Add an asset and start a scan."
      contentClassName="flex min-h-0 flex-1 flex-col gap-4"
      spotlight={<DashboardPanelSpotlight className="right-[-3rem] top-[-2rem]" size="sm" />}
      beam
    >
        <form className="space-y-3" onSubmit={onSubmit}>
          <Input
            value={assetForm.name}
            onChange={(event) =>
              onAssetFormChange({ ...assetForm, name: event.target.value })
            }
            placeholder="Production API"
            required
            className="h-11 rounded-2xl border-white/10 bg-white/[0.03] px-4 text-white placeholder:text-slate-500 focus-visible:border-emerald-400/30 focus-visible:ring-emerald-400/20"
          />
          <Input
            value={assetForm.domain}
            onChange={(event) =>
              onAssetFormChange({ ...assetForm, domain: event.target.value })
            }
            placeholder="example.com"
            required
            className="h-11 rounded-2xl border-white/10 bg-white/[0.03] px-4 text-white placeholder:text-slate-500 focus-visible:border-emerald-400/30 focus-visible:ring-emerald-400/20"
          />
          <Select
            value={assetForm.type}
            onChange={(event) =>
              onAssetFormChange({
                ...assetForm,
                type: Number(event.target.value) as AssetType,
              })
            }
          >
            <option className="bg-slate-950" value={1}>Domain</option>
            <option className="bg-slate-950" value={2}>IP</option>
            <option className="bg-slate-950" value={3}>Web App</option>
          </Select>

          <Button
            type="submit"
            disabled={isSubmitting}
            className="h-11 w-full rounded-2xl bg-gradient-to-r from-emerald-400 via-emerald-500 to-green-400 font-semibold text-[#032212] hover:brightness-105"
          >
            {isSubmitting ? "Saving..." : "Create asset"}
          </Button>
        </form>

        {feedback ? (
          <div className="rounded-[18px] border border-emerald-400/14 bg-emerald-400/10 px-4 py-2.5 text-sm text-emerald-100">
            {feedback}
          </div>
        ) : null}

        <Separator className="bg-white/8" />

        <ScrollArea className="space-y-3">
          {isLoading ? (
            <DashboardPanelNotice message="Assets are loading..." compact />
          ) : assets.length === 0 ? (
            <DashboardPanelNotice message="No assets yet." compact />
          ) : (
            assets.map((asset) => (
              <DashboardEntityCard key={asset.id}>
                <DashboardEntityHeader
                  title={asset.name}
                  subtitle={asset.domain}
                  action={<Badge variant="outline">{assetTypeLabels[asset.type]}</Badge>}
                />

                <Button
                  type="button"
                  onClick={() => onStartScan(asset)}
                  disabled={isSubmitting}
                  className="h-10 w-full rounded-2xl bg-white/[0.06] text-white hover:bg-white/[0.1]"
                >
                  <TerminalSquare className="mr-2 size-4" />
                  Start scan
                </Button>
              </DashboardEntityCard>
            ))
          )}
        </ScrollArea>
    </DashboardPanelCard>
  )
}
