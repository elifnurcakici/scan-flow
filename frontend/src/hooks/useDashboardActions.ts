import { startTransition, useState } from "react"
import { useMutation, useQueryClient } from "@tanstack/react-query"

import { http } from "@/lib/api"
import { useScanSocket } from "@/sockets/useScanSocket"
import type { ApiResponse, Asset, AssetType } from "@/types/api"

export type CreateAssetRequest = {
  name: string
  domain: string
  type: AssetType
}

type StartScanResponse = {
  message: string
  scanId: number
}

export function useDashboardActions({
  onSelectedScanChange,
}: {
  onSelectedScanChange: (scanId: number) => void
}) {
  const queryClient = useQueryClient()
  const [assetForm, setAssetForm] = useState<CreateAssetRequest>({
    name: "",
    domain: "",
    type: 1,
  })
  const [feedback, setFeedback] = useState<string | null>(null)

  const createAssetMutation = useMutation({
    mutationFn: async (payload: CreateAssetRequest) => {
      const response = await http.post<ApiResponse<Asset>>("/api/assets", payload)
      return response.data.data
    },
    onSuccess: async () => {
      setAssetForm({ name: "", domain: "", type: 1 })
      setFeedback("Asset created.")
      await queryClient.invalidateQueries({ queryKey: ["assets"] })
    },
  })

  const startScanMutation = useMutation({
    mutationFn: async (asset: Asset) => {
      const response = await http.post<ApiResponse<StartScanResponse>>("/api/scans/start", {
        name: `${asset.name} Baseline Scan`,
        assetId: asset.id,
      })
      return response.data.data
    },
    onSuccess: async (payload) => {
      setFeedback("Scan started.")
      await queryClient.invalidateQueries({ queryKey: ["scans"] })
      await queryClient.invalidateQueries({ queryKey: ["assets"] })

      if (payload?.scanId) {
        startTransition(() => onSelectedScanChange(payload.scanId))
      }
    },
  })

  useScanSocket({
    onScanUpdated: (scanId) => {
      void queryClient.invalidateQueries({ queryKey: ["scans"] })
      void queryClient.invalidateQueries({ queryKey: ["scan-detail", scanId] })
    },
  })

  async function handleAssetSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setFeedback(null)
    await createAssetMutation.mutateAsync(assetForm)
  }

  function selectScan(scanId: number) {
    startTransition(() => onSelectedScanChange(scanId))
  }

  function startScan(asset: Asset) {
    void startScanMutation.mutateAsync(asset)
  }

  return {
    assetForm,
    feedback,
    isSubmitting: createAssetMutation.isPending || startScanMutation.isPending,
    setAssetForm,
    handleAssetSubmit,
    startScan,
    selectScan,
  }
}
