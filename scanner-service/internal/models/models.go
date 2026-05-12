package models

type ScanCreatedEvent struct {
	ScanId      string `json:"scanId"`
	AssetId     string `json:"assetId"`
	ScannerId   string `json:"scannerId"`
	ScannerName string `json:"scannerName"`
	ScannerType string `json:"scannerType"`
	Domain      string `json:"domain"`
	AssetType   string `json:"assetType"`
}

type Vulnerability struct {
	Type           string   `json:"type"`
	Severity       string   `json:"severity"`
	Description    string   `json:"description"`
	CweId          string   `json:"cweId,omitempty"`
	CvssScore      *float64 `json:"cvssScore,omitempty"`
	CvssVector     string   `json:"cvssVector,omitempty"`
	Recommendation string   `json:"recommendation,omitempty"`
}

type ScanResultEvent struct {
	ScanId          string          `json:"scanId"`
	AssetId         string          `json:"assetId"`
	ScannerId       string          `json:"scannerId"`
	Status          string          `json:"status"`
	Vulnerabilities []Vulnerability `json:"vulnerabilities,omitempty"`
	ErrorReason     string          `json:"errorReason,omitempty"`
}
