package producer

import(
	"encoding/json"

	"github.com/IBM/sarama"
)

type KafkaProducer struct{
	producer sarama.SyncProducer
	topic string
}

func NewKafkaProducer(brokers []string, topic string) (*KafkaProducer, error){
	config := sarama.NewConfig()
	config.Producer.Return.Successes = true

	producer, err := sarama.NewSyncProducer(brokers, config)
	if err != nil {
		return nil, err
	}

	return &KafkaProducer{
		producer: producer,
		topic: topic,
	},nil
}

func (kp *KafkaProducer) Publish(message interface{}) error {
	jsonMessage, err := json.Marshal(message)
	if err != nil{
		return err
	}

	msg := &sarama.ProducerMessage{
		Topic : kp.topic,
		Value: sarama.StringEncoder(jsonMessage),
	}

	_,_, err = kp.producer.SendMessage(msg)
	return err
}

func (kp *KafkaProducer) Close() error{
	return kp.producer.Close()
}