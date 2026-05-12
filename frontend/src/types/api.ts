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
  id: string
  email: string
  createdAt: string
  updatedAt: string
}

export type AssetType = 1 | 2 | 3 | 4

export type Asset = {
  id: string
  name: string
  domain: string
  type: AssetType
  createdAt: string
  updatedAt: string
  scanCount: number
  vulnerabilityCount: number
  criticalCount: number
  highCount: number
}

export type ScanListItem = {
  id: string
  name: string
  assetId: string
  assetName: string
  scannerName: string
  scannerType: string
  status: string
  errorReason: string | null
  vulnerabilityCount: number
  criticalCount: number
  highCount: number
  createdAt: string
}

export type ScanHistoryItem = {
  id: string
  status: string
  createdAt: string
}

export type VulnerabilityItem = {
  id: string
  severity: string
  type: string
  description: string
  cweId: string | null
  cvssScore: number | null
  cvssVector: string | null
  recommendation: string | null
  createdAt: string
}

export type ScanDetail = {
  id: string
  name: string
  assetId: string
  assetName: string
  assetDomain: string
  scannerName: string
  scannerType: string
  status: string
  errorReason: string | null
  createdAt: string
  history: ScanHistoryItem[]
  vulnerabilities: VulnerabilityItem[]
}

export type VulnerabilityListItem = VulnerabilityItem & {
  scanId: string
  scanName: string
  assetId: string
  assetName: string
  assetDomain: string
}
