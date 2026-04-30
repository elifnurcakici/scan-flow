namespace backend.Services.Interfaces;

public interface IKafkaProducerService
{
    Task ProducerAsync<T>(string topic, T message);
}
