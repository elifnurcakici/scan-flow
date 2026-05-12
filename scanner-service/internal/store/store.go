package store

import (
	"crypto/rand"
	"database/sql"
	"encoding/hex"
	"fmt"
	"time"

	_ "github.com/lib/pq"
	"scanflow.scanner/internal/models"
)

type Store struct {
	db *sql.DB
}

func New(dsn string) (*Store, error) {
	db, err := sql.Open("postgres", dsn)
	if err != nil {
		return nil, err
	}

	db.SetMaxOpenConns(10)
	db.SetMaxIdleConns(5)
	db.SetConnMaxLifetime(30 * time.Minute)

	if err := db.Ping(); err != nil {
		return nil, err
	}

	return &Store{db: db}, nil
}

func (s *Store) Close() error {
	return s.db.Close()
}

func (s *Store) MarkScanRunning(scanID string) error {
	return s.updateScanStatus(scanID, "Running", "")
}

func (s *Store) MarkScanFailed(scanID string, errorReason string) error {
	return s.updateScanStatus(scanID, "Failed", errorReason)
}

func (s *Store) MarkScanFinished(scanID string, vulnerabilities []models.Vulnerability) error {
	tx, err := s.db.Begin()
	if err != nil {
		return err
	}

	defer func() {
		if err != nil {
			_ = tx.Rollback()
		}
	}()

	if _, err = tx.Exec(
		`UPDATE "Scan" SET "Status" = $1, "ErrorReason" = NULL WHERE "Id" = $2`,
		3,
		scanID,
	); err != nil {
		return err
	}

	if _, err = tx.Exec(
		`INSERT INTO "ScanHistory" ("Id", "ScanId", "Status", "CreatedAt") VALUES ($1, $2, $3, $4)`,
		newUUID(),
		scanID,
		3,
		time.Now().UTC(),
	); err != nil {
		return err
	}

	for _, vulnerability := range vulnerabilities {
		if _, err = tx.Exec(
			`INSERT INTO "Vulnerability" ("Id", "ScanId", "Severity", "Type", "Description", "CweId", "CvssScore", "CvssVector", "Recommendation", "CreatedAt")
             VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10)`,
			newUUID(),
			scanID,
			vulnerability.Severity,
			vulnerability.Type,
			vulnerability.Description,
			nullIfEmpty(vulnerability.CweId),
			vulnerability.CvssScore,
			nullIfEmpty(vulnerability.CvssVector),
			nullIfEmpty(vulnerability.Recommendation),
			time.Now().UTC(),
		); err != nil {
			return err
		}
	}

	return tx.Commit()
}

func (s *Store) updateScanStatus(scanID string, status string, errorReason string) error {
	statusValue, err := mapStatus(status)
	if err != nil {
		return err
	}

	tx, err := s.db.Begin()
	if err != nil {
		return err
	}

	defer func() {
		if err != nil {
			_ = tx.Rollback()
		}
	}()

	var nullableError any
	if errorReason == "" {
		nullableError = nil
	} else {
		nullableError = limitText(errorReason, 500)
	}

	if _, err = tx.Exec(
		`UPDATE "Scan" SET "Status" = $1, "ErrorReason" = $2 WHERE "Id" = $3`,
		statusValue,
		nullableError,
		scanID,
	); err != nil {
		return err
	}

	if _, err = tx.Exec(
		`INSERT INTO "ScanHistory" ("Id", "ScanId", "Status", "CreatedAt") VALUES ($1, $2, $3, $4)`,
		newUUID(),
		scanID,
		statusValue,
		time.Now().UTC(),
	); err != nil {
		return err
	}

	return tx.Commit()
}

func mapStatus(status string) (int, error) {
	switch status {
	case "Pending":
		return 1, nil
	case "Running":
		return 2, nil
	case "Finished":
		return 3, nil
	case "Failed":
		return 4, nil
	default:
		return 0, fmt.Errorf("unsupported status: %s", status)
	}
}

func nullIfEmpty(value string) any {
	if value == "" {
		return nil
	}

	return value
}

func newUUID() string {
	bytes := make([]byte, 16)
	_, _ = rand.Read(bytes)
	bytes[6] = (bytes[6] & 0x0f) | 0x40
	bytes[8] = (bytes[8] & 0x3f) | 0x80

	hexValue := hex.EncodeToString(bytes)
	return fmt.Sprintf(
		"%s-%s-%s-%s-%s",
		hexValue[0:8],
		hexValue[8:12],
		hexValue[12:16],
		hexValue[16:20],
		hexValue[20:32],
	)
}

func limitText(value string, max int) string {
	if max <= 0 {
		return value
	}

	runes := []rune(value)
	if len(runes) <= max {
		return value
	}

	return string(runes[:max])
}
