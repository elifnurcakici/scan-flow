export type ApiResponse<T> = {
  success: boolean
  data: T | null
  message: string | null
  errors: string[] | null
  traceId: string | null
}

export type Session = {
  accessToken: string
  refreshToken: string
  email: string
}

export type AuthPayload = {
  accessToken: string
  refreshToken: string
  email: string
  accessTokenExpiresAt: string
  refreshTokenExpiresAt: string
}

export type UserProfile = {
  id: number
  email: string
  createdAt: string
  updatedAt: string
}

export type AssetType = 1 | 2 | 3

export type Asset = {
  id: number
  name: string
  domain: string
  type: AssetType
  createdAt: string
  updatedAt: string
}

export type ScanListItem = {
  id: number
  name: string
  assetId: number
  assetName: string
  status: string
  errorReason: string | null
  vulnerabilityCount: number
  createdAt: string
}

export type ScanHistoryItem = {
  id: number
  status: string
  createdAt: string
}

export type VulnerabilityItem = {
  id: number
  severity: string
  type: string
  description: string
  createdAt: string
}

export type ScanDetail = {
  id: number
  name: string
  assetId: number
  assetName: string
  assetDomain: string
  status: string
  errorReason: string | null
  createdAt: string
  history: ScanHistoryItem[]
  vulnerabilities: VulnerabilityItem[]
}
