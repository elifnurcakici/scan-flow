package main

import (
	"log"
	"os"
	"strings"

	"scanflow.scanner/internal/consumer"
	"scanflow.scanner/internal/producer"
	"scanflow.scanner/internal/scanner"
	"scanflow.scanner/internal/store"
)

func main() {
	brokers := getEnvList("KAFKA_BROKERS", []string{"localhost:9092"})
	dsn := getEnv("SCANNER_DB_DSN", "host=localhost port=5432 user=scanflow_user password=843038 dbname=scanflow_db sslmode=disable")

	consumeTopic := getEnv("KAFKA_CONSUME_TOPIC", "scan-created")
	produceTopic := getEnv("KAFKA_PRODUCE_TOPIC", "scan-results")
	profile := scanner.Profile(getEnv("SCANNER_PROFILE", string(scanner.ProfileDast)))

	if err := scanner.ValidateToolAvailability(profile); err != nil {
		log.Fatalf("Scanner tool is not available: %v", err)
	}
	if err := scanner.PrepareRuntime(profile); err != nil {
		log.Fatalf("Scanner runtime preparation failed: %v", err)
	}

	dbStore, err := store.New(dsn)
	if err != nil {
		log.Fatalf("Database bağlantısı kurulamadı: %v", err)
	}
	defer dbStore.Close()

	prod, err := producer.NewKafkaProducer(brokers, produceTopic)
	if err != nil {
		log.Fatalf("Producer başlatılamadı: %v", err)
	}
	defer prod.Close()

	cons := consumer.NewConsumer(prod, dbStore, profile)
	cons.Start(brokers, consumeTopic)
}

func getEnv(key, fallback string) string {
	value := os.Getenv(key)
	if value == "" {
		return fallback
	}
	return value
}

func getEnvList(key string, fallback []string) []string {
	value := os.Getenv(key)
	if value == "" {
		return fallback
	}

	parts := strings.Split(value, ",")
	brokers := make([]string, 0, len(parts))

	for _, part := range parts {
		trimmed := strings.TrimSpace(part)
		if trimmed != "" {
			brokers = append(brokers, trimmed)
		}
	}

	if len(brokers) == 0 {
		return fallback
	}

	return brokers
}
