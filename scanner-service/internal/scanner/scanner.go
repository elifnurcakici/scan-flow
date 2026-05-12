package scanner

import (
	"bufio"
	"bytes"
	"context"
	"encoding/json"
	"errors"
	"fmt"
	"os"
	"os/exec"
	"strings"
	"time"

	"scanflow.scanner/internal/models"
)

type Profile string

const (
	ProfileDast Profile = "dast"
	ProfileSca  Profile = "sca"
)

func Run(profile Profile, event models.ScanCreatedEvent) models.ScanResultEvent {
	ctx, cancel := context.WithTimeout(context.Background(), 2*time.Minute)
	defer cancel()

	switch profile {
	case ProfileDast:
		return runNuclei(ctx, event)
	case ProfileSca:
		return runTrivyRepo(ctx, event)
	default:
		return failedResult(event, fmt.Sprintf("unsupported scanner profile: %s", profile))
	}
}

func runNuclei(ctx context.Context, event models.ScanCreatedEvent) models.ScanResultEvent {
	target := normalizeNucleiTarget(event.Domain)

	cmd := exec.CommandContext(
		ctx,
		"nuclei",
		"-u", target,
		"-as",
		"-t", "/root/nuclei-templates",
		"-jsonl",
		"-silent",
		"-duc",
		"-rl", "50",
	)

	output, err := cmd.CombinedOutput()
	if err != nil {
		return failedResult(event, fmt.Sprintf("nuclei execution failed: %s", readCommandError(output, err)))
	}

	vulnerabilities, parseErr := parseNucleiJSONL(string(output))
	if parseErr != nil {
		return failedResult(event, fmt.Sprintf("nuclei output could not be parsed: %s", parseErr.Error()))
	}

	return models.ScanResultEvent{
		ScanId:          event.ScanId,
		AssetId:         event.AssetId,
		ScannerId:       event.ScannerId,
		Status:          "Finished",
		Vulnerabilities: vulnerabilities,
	}
}

func runTrivyRepo(ctx context.Context, event models.ScanCreatedEvent) models.ScanResultEvent {
	cmd := exec.CommandContext(
		ctx,
		"trivy",
		"repo",
		"--format", "json",
		"--quiet",
		"--scanners", "vuln,misconfig,secret",
		event.Domain,
	)

	var stderr bytes.Buffer
	cmd.Stderr = &stderr

	output, err := cmd.Output()
	if err != nil {
		return failedResult(event, fmt.Sprintf("trivy execution failed: %s", readCommandError(stderr.Bytes(), err)))
	}

	vulnerabilities, parseErr := parseTrivyJSON(output)
	if parseErr != nil {
		return failedResult(event, fmt.Sprintf("trivy output could not be parsed: %s", parseErr.Error()))
	}

	return models.ScanResultEvent{
		ScanId:          event.ScanId,
		AssetId:         event.AssetId,
		ScannerId:       event.ScannerId,
		Status:          "Finished",
		Vulnerabilities: vulnerabilities,
	}
}

func failedResult(event models.ScanCreatedEvent, reason string) models.ScanResultEvent {
	return models.ScanResultEvent{
		ScanId:      event.ScanId,
		AssetId:     event.AssetId,
		ScannerId:   event.ScannerId,
		Status:      "Failed",
		ErrorReason: reason,
	}
}

func readCommandError(output []byte, err error) string {
	trimmed := strings.TrimSpace(string(output))
	if trimmed == "" {
		return err.Error()
	}

	return trimmed
}

func normalizeNucleiTarget(target string) string {
	return strings.TrimSpace(target)
}

type nucleiResult struct {
	Info struct {
		Name           string `json:"name"`
		Severity       string `json:"severity"`
		Description    string `json:"description"`
		Reference      any    `json:"reference"`
		Classification struct {
			CweID      any     `json:"cwe-id"`
			CvssScore  float64 `json:"cvss-score"`
			CvssMetric string  `json:"cvss-metrics"`
		} `json:"classification"`
	} `json:"info"`
	TemplateID string `json:"template-id"`
	MatcherName string `json:"matcher-name"`
}

func parseNucleiJSONL(raw string) ([]models.Vulnerability, error) {
	scanner := bufio.NewScanner(strings.NewReader(raw))
	findings := make([]models.Vulnerability, 0)

	for scanner.Scan() {
		line := strings.TrimSpace(scanner.Text())
		if line == "" {
			continue
		}

		var entry nucleiResult
		if err := json.Unmarshal([]byte(line), &entry); err != nil {
			return nil, err
		}

		vulnerabilityType := firstNonEmpty(entry.Info.Name, entry.TemplateID, entry.MatcherName, "Nuclei finding")
		description := firstNonEmpty(entry.Info.Description, "Detected by nuclei templates.")
		recommendation := "Review the matched nuclei template and validate whether the exposure is reachable in the target environment."
		severity := normalizeSeverity(entry.Info.Severity)
			cweID := resolveCWE(entry.Info.Classification.CweID)
		var cvssScore *float64
		if entry.Info.Classification.CvssScore > 0 {
			score := entry.Info.Classification.CvssScore
			cvssScore = &score
		}

		findings = append(findings, models.Vulnerability{
			Type:           limitText(vulnerabilityType, 100),
			Severity:       severity,
			Description:    limitText(description, 500),
			CweId:          limitText(cweID, 50),
			CvssScore:      cvssScore,
			CvssVector:     limitText(emptyIfBlank(entry.Info.Classification.CvssMetric), 128),
			Recommendation: limitText(recommendation, 1000),
		})
	}

	if err := scanner.Err(); err != nil {
		return nil, err
	}

	return findings, nil
}

type trivyReport struct {
	Results []struct {
		Target            string `json:"Target"`
		Vulnerabilities   []struct {
			VulnerabilityID  string             `json:"VulnerabilityID"`
			PkgName          string             `json:"PkgName"`
			Title            string             `json:"Title"`
			Description      string             `json:"Description"`
			Severity         string             `json:"Severity"`
			PrimaryURL       string             `json:"PrimaryURL"`
			FixedVersion     string             `json:"FixedVersion"`
			CweIDs           []string           `json:"CweIDs"`
			CVSS             map[string]struct {
				V3Score  float64 `json:"V3Score"`
				V3Vector string  `json:"V3Vector"`
			} `json:"CVSS"`
		} `json:"Vulnerabilities"`
		Misconfigurations []struct {
			ID          string `json:"ID"`
			Title       string `json:"Title"`
			Description string `json:"Description"`
			Severity    string `json:"Severity"`
			Resolution  string `json:"Resolution"`
		} `json:"Misconfigurations"`
		Secrets []struct {
			RuleID   string `json:"RuleID"`
			Title    string `json:"Title"`
			Severity string `json:"Severity"`
			Match    string `json:"Match"`
		} `json:"Secrets"`
	} `json:"Results"`
}

func parseTrivyJSON(raw []byte) ([]models.Vulnerability, error) {
	var report trivyReport
	if err := json.Unmarshal(raw, &report); err != nil {
		return nil, err
	}

	findings := make([]models.Vulnerability, 0)
	for _, result := range report.Results {
		for _, vulnerability := range result.Vulnerabilities {
			cweID := ""
			if len(vulnerability.CweIDs) > 0 {
				cweID = vulnerability.CweIDs[0]
			}

			cvssScore, cvssVector := resolveTrivyCVSS(vulnerability.CVSS)
			recommendation := "Upgrade or replace the affected dependency."
			if vulnerability.FixedVersion != "" {
				recommendation = fmt.Sprintf("Upgrade %s to %s or later.", vulnerability.PkgName, vulnerability.FixedVersion)
			}

			findings = append(findings, models.Vulnerability{
				Type:           limitText(firstNonEmpty(vulnerability.Title, vulnerability.VulnerabilityID, "Trivy vulnerability"), 100),
				Severity:       normalizeSeverity(vulnerability.Severity),
				Description:    limitText(firstNonEmpty(vulnerability.Description, fmt.Sprintf("Dependency issue detected in %s.", result.Target)), 500),
				CweId:          limitText(emptyIfBlank(cweID), 50),
				CvssScore:      cvssScore,
				CvssVector:     limitText(emptyIfBlank(cvssVector), 128),
				Recommendation: limitText(recommendation, 1000),
			})
		}

		for _, misconfiguration := range result.Misconfigurations {
			findings = append(findings, models.Vulnerability{
				Type:           limitText(firstNonEmpty(misconfiguration.Title, misconfiguration.ID, "Trivy misconfiguration"), 100),
				Severity:       normalizeSeverity(misconfiguration.Severity),
				Description:    limitText(firstNonEmpty(misconfiguration.Description, "Repository misconfiguration detected by Trivy."), 500),
				CweId:          "",
				CvssScore:      nil,
				CvssVector:     "",
				Recommendation: limitText(emptyIfBlank(firstNonEmpty(misconfiguration.Resolution, "Apply the recommended configuration hardening.")), 1000),
			})
		}

		for _, secret := range result.Secrets {
			findings = append(findings, models.Vulnerability{
				Type:           limitText(firstNonEmpty(secret.Title, secret.RuleID, "Exposed secret"), 100),
				Severity:       normalizeSeverity(secret.Severity),
				Description:    limitText(fmt.Sprintf("Potential secret material detected in %s.", result.Target), 500),
				CweId:          "",
				CvssScore:      nil,
				CvssVector:     "",
				Recommendation: limitText("Rotate the exposed secret and remove it from the repository history if necessary.", 1000),
			})
		}
	}

	return findings, nil
}

func resolveTrivyCVSS(cvss map[string]struct {
	V3Score  float64 `json:"V3Score"`
	V3Vector string  `json:"V3Vector"`
}) (*float64, string) {
	for _, value := range cvss {
		if value.V3Score > 0 {
			score := value.V3Score
			return &score, value.V3Vector
		}
	}

	return nil, ""
}

func resolveCWE(value any) string {
	switch typed := value.(type) {
	case []any:
		for _, item := range typed {
			if stringValue, ok := item.(string); ok && strings.TrimSpace(stringValue) != "" {
				return stringValue
			}
		}
	case []string:
		if len(typed) > 0 && strings.TrimSpace(typed[0]) != "" {
			return typed[0]
		}
	case string:
		if strings.TrimSpace(typed) != "" {
			return typed
		}
	}

	return ""
}

func normalizeSeverity(value string) string {
	switch strings.ToLower(strings.TrimSpace(value)) {
	case "critical":
		return "Critical"
	case "high":
		return "High"
	case "medium":
		return "Medium"
	case "low":
		return "Low"
	default:
		return "Medium"
	}
}

func firstNonEmpty(values ...string) string {
	for _, value := range values {
		if strings.TrimSpace(value) != "" {
			return value
		}
	}

	return ""
}

func emptyIfBlank(value string) string {
	if strings.TrimSpace(value) == "" {
		return ""
	}

	return value
}

func limitText(value string, max int) string {
	trimmed := strings.TrimSpace(value)
	if max <= 0 || len([]rune(trimmed)) <= max {
		return trimmed
	}

	return string([]rune(trimmed)[:max])
}

func ValidateToolAvailability(profile Profile) error {
	switch profile {
	case ProfileDast:
		return ensureBinary("nuclei")
	case ProfileSca:
		return ensureBinary("trivy")
	default:
		return errors.New("unsupported scanner profile")
	}
}

func PrepareRuntime(profile Profile) error {
	switch profile {
	case ProfileDast:
		return ensureNucleiTemplates()
	case ProfileSca:
		return nil
	default:
		return errors.New("unsupported scanner profile")
	}
}

func ensureBinary(name string) error {
	_, err := exec.LookPath(name)
	if err != nil {
		return fmt.Errorf("%s is not installed", name)
	}

	return nil
}

func ensureNucleiTemplates() error {
	const templateDir = "/root/nuclei-templates"

	if entries, err := os.ReadDir(templateDir); err == nil && len(entries) > 0 {
		return nil
	}

	cmd := exec.Command("nuclei", "-ut", "-ud", templateDir)
	output, err := cmd.CombinedOutput()
	if err != nil {
		return fmt.Errorf("nuclei templates could not be prepared: %s", readCommandError(output, err))
	}

	return nil
}
