using System.Text.Json;
using Confluent.Kafka;

using backend.Services.Interfaces;

namespace backend.Services
{
    public class KafkaProducerService : IKafkaProducerService
    {
        private readonly IProducer<Null, string> _producer;
        public KafkaProducerService(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task ProducerAsync<T>(string topic, T message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);

            await _producer.ProduceAsync(topic, new Message<Null, string>
            {
                Value = jsonMessage
            });
        }
    }
}
