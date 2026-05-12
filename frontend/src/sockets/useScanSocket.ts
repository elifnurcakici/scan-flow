import { useEffect } from 'react'
import { baseURL } from '../lib/api'

type Options = {
  onScanUpdated: (payload: { scanId: string; status?: string; assetId?: string }) => void
}

export function useScanSocket({ onScanUpdated }: Options) {
  useEffect(() => {
    const socketUrl = baseURL.replace('http://', 'ws://').replace('https://', 'wss://')
    const socket = new WebSocket(`${socketUrl}/ws/scans`)
    let pingInterval: number | undefined

    socket.onopen = () => {
      pingInterval = window.setInterval(() => {
        if (socket.readyState === WebSocket.OPEN) {
          socket.send('ping')
        }
      }, 25000)
    }

    socket.onmessage = (event) => {
      if (event.data === 'pong') {
        return
      }

      try {
        const payload = JSON.parse(String(event.data)) as {
          type?: string
          scanId?: string
          status?: string
          assetId?: string
        }

        if (payload.type === 'scan_updated' && typeof payload.scanId === 'string') {
          onScanUpdated({
            scanId: payload.scanId,
            status: payload.status,
            assetId: payload.assetId,
          })
        }
      } catch {
      }
    }

    return () => {
      if (pingInterval) {
        window.clearInterval(pingInterval)
      }
      socket.close()
    }
  }, [onScanUpdated])
}
