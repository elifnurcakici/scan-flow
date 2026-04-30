package consumer

import (
	"encoding/json"
	"log"

	"github.com/IBM/sarama"
	"scanflow.scanner/internal/models"
	"scanflow.scanner/internal/producer"
	"scanflow.scanner/internal/scanner"
	"scanflow.scanner/internal/store"
)

type Consumer struct {
	producer *producer.KafkaProducer
	store    *store.Store
}

func NewConsumer(prod *producer.KafkaProducer, dbStore *store.Store) *Consumer {
	return &Consumer{
		producer: prod,
		store:    dbStore,
	}
}

func (c *Consumer) Start(brokers []string, topic string) {
	config := sarama.NewConfig()
	config.Consumer.Return.Errors = true

	consumer, err := sarama.NewConsumer(brokers, config)
	if err != nil {
		log.Fatalf("Kafka consumer error: %v", err)
	}

	partitionConsumer, err := consumer.ConsumePartition(topic, 0, sarama.OffsetNewest)
	if err != nil {
		log.Fatalf("Partition consumer error: %v", err)
	}

	log.Println("Scanner Service Kafka listens...")
	for message := range partitionConsumer.Messages() {
		var event models.ScanCreatedEvent
		if err := json.Unmarshal(message.Value, &event); err != nil {
			log.Println("JSON parse hatası:", err)
			continue
		}

		log.Printf("Scan Received : %+v\n", event)

		if err := c.store.MarkScanRunning(event.ScanId); err != nil {
			log.Println("Scan could not be marked as running:", err)
			continue
		}

		runningEvent := models.ScanResultEvent{
			ScanId:  event.ScanId,
			AssetId: event.AssetId,
			Status:  "Running",
		}

		if err := c.producer.Publish(runningEvent); err != nil {
			log.Println("Running event could not be sent: ", err)
			continue
		}

		result := scanner.SimulateScan(event.ScanId, event.AssetId)

		switch result.Status {
		case "Failed":
			if err := c.store.MarkScanFailed(event.ScanId, result.ErrorReason); err != nil {
				log.Println("Scan could not be marked as failed:", err)
				continue
			}
		case "Finished":
			if err := c.store.MarkScanFinished(event.ScanId, result.Vulnerabilities); err != nil {
				log.Println("Scan could not be finished in database:", err)
				continue
			}
		default:
			log.Println("Unsupported scan result status:", result.Status)
			continue
		}

		if err := c.producer.Publish(result); err != nil {
			log.Println("Result could not be sent: ", err)
		} else {
			log.Println("Scan results were sent to Kafka.")
		}
	}
}
