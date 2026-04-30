package scanner

import (
	"math/rand"
	"time"

	"scanflow.scanner/internal/models"
)

var severities = []string{"Low", "Medium", "High", "Critical"}
var vulnerabilityTypes = []string{
	"SQL Injection",
	"Cross-site Scripting (XSS)",
	"Security Misconfiguration",
	"Broken Authentication",
}

func SimulateScan(scanId int64, assetId int64) models.ScanResultEvent {
	rand.Seed(time.Now().UnixNano())

	time.Sleep(3 * time.Second)

	success := rand.Intn(100) < 80

	if !success {
		return models.ScanResultEvent{
			ScanId:      scanId,
			AssetId:     assetId,
			Status:      "Failed",
			ErrorReason: "Scan failed fur to network timeout",
		}
	}

	vulnCount := rand.Intn(3) + 1
	vulnerabilities := make([]models.Vulnerability, vulnCount)

	for i := 0; i < vulnCount; i++ {
		vulnerabilities[i] = models.Vulnerability{
			Type:        vulnerabilityTypes[rand.Intn(len(vulnerabilityTypes))],
			Severity:    severities[rand.Intn(len(severities))],
			Description: "Simulated vulnerability detected",
		}
	}

	return models.ScanResultEvent{
		ScanId:          scanId,
		AssetId:         assetId,
		Status:          "Finished",
		Vulnerabilities: vulnerabilities,
	}
}
