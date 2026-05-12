using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace backend.Services.Kafka;

public class KafkaTopicInitializerService : IHostedService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaTopicInitializerService> _logger;

    public KafkaTopicInitializerService(
        IConfiguration configuration,
        ILogger<KafkaTopicInitializerService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var bootstrapServers = _configuration["Kafka:BootstrapServers"];
        var topics = new[]
        {
            _configuration["Kafka:DastScanTopic"],
            _configuration["Kafka:ScaScanTopic"],
            _configuration["Kafka:ScanResultTopic"]
        }
        .Where(topic => !string.IsNullOrWhiteSpace(topic))
        .Cast<string>()
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToArray();

        if (string.IsNullOrWhiteSpace(bootstrapServers) || topics.Length == 0)
        {
            _logger.LogWarning("Kafka topic initialization skipped because configuration is incomplete.");
            return;
        }

        using var adminClient = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        }).Build();

        const int maxAttempts = 10;

        for (var attempt = 1; attempt <= maxAttempts && !cancellationToken.IsCancellationRequested; attempt++)
        {
            try
            {
                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
                var existingTopics = metadata.Topics
                    .Select(topic => topic.Topic)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var missingTopics = topics
                    .Where(topic => !existingTopics.Contains(topic))
                    .Select(topic => new TopicSpecification
                    {
                        Name = topic,
                        NumPartitions = 1,
                        ReplicationFactor = 1
                    })
                    .ToList();

                if (missingTopics.Count == 0)
                {
                    _logger.LogInformation("Kafka topics are ready.");
                    return;
                }

                await adminClient.CreateTopicsAsync(missingTopics);
                _logger.LogInformation(
                    "Kafka topics created: {Topics}",
                    string.Join(", ", missingTopics.Select(topic => topic.Name)));
                return;
            }
            catch (CreateTopicsException ex) when (ex.Results.All(result =>
                         result.Error.Code == ErrorCode.TopicAlreadyExists))
            {
                _logger.LogInformation("Kafka topics already exist.");
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger.LogWarning(
                    ex,
                    "Kafka topics could not be initialized on attempt {Attempt}/{MaxAttempts}. Retrying...",
                    attempt,
                    maxAttempts);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }

        _logger.LogWarning("Kafka topic initialization finished without a confirmed success.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
