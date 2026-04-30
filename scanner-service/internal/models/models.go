package models

type ScanCreatedEvent struct {
	ScanId    int64  `json:"scanId"`
	AssetId   int64  `json:"assetId"`
	Domain    string `json:"domain"`
	AssetType string `json:"assetType"`
}

type Vulnerability struct {
	Type        string `json:"type"`
	Severity    string `json:"severity"`
	Description string `json:"description"`
}

type ScanResultEvent struct {
	ScanId          int64           `json:"scanId"`
	AssetId         int64           `json:"assetId"`
	Status          string          `json:"status"`
	Vulnerabilities []Vulnerability `json:"vulnerabilities,omitempty"`
	ErrorReason     string          `json:"errorReason,omitempty"`
}
