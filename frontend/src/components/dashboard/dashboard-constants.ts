import type { AssetType } from "@/types/api"

export const assetTypeLabels: Record<AssetType, string> = {
  1: "Domain",
  2: "IP",
  3: "Web App",
}

export const detectionSeries = [22, 34, 48, 30, 56, 67, 51, 72, 59]

export const statusPillClass = {
  Running: "secondary" as const,
  Finished: "default" as const,
  Failed: "destructive" as const,
  Pending: "outline" as const,
}
