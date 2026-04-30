package store

import (
	"database/sql"
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

func (s *Store) MarkScanRunning(scanID int64) error {
	return s.updateScanStatus(scanID, "Running", "")
}

func (s *Store) MarkScanFailed(scanID int64, errorReason string) error {
	return s.updateScanStatus(scanID, "Failed", errorReason)
}

func (s *Store) MarkScanFinished(scanID int64, vulnerabilities []models.Vulnerability) error {
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
		`INSERT INTO "ScanHistory" ("ScanId", "Status", "CreatedAt") VALUES ($1, $2, $3)`,
		scanID,
		3,
		time.Now().UTC(),
	); err != nil {
		return err
	}

	for _, vulnerability := range vulnerabilities {
		if _, err = tx.Exec(
			`INSERT INTO "Vulnerability" ("ScanId", "Severity", "Type", "Description", "CreatedAt") VALUES ($1, $2, $3, $4, $5)`,
			scanID,
			vulnerability.Severity,
			vulnerability.Type,
			vulnerability.Description,
			time.Now().UTC(),
		); err != nil {
			return err
		}
	}

	return tx.Commit()
}

func (s *Store) updateScanStatus(scanID int64, status string, errorReason string) error {
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
		nullableError = errorReason
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
		`INSERT INTO "ScanHistory" ("ScanId", "Status", "CreatedAt") VALUES ($1, $2, $3)`,
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
