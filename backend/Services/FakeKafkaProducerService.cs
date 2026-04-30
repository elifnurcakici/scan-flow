using backend.Services.Interfaces;

namespace backend.Services;

public class FakeKafkaProducerService : IKafkaProducerService
{
    public Task ProducerAsync<T>(string topic, T message)
    {
        return Task.CompletedTask;
    }
}
