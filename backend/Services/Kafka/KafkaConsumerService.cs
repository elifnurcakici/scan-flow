using System.Text.Json;
using Confluent.Kafka;
using backend.Events;
using backend.Services.WebSockets;

namespace backend.Services.Kafka
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly WebSocketNotificationService _notificationService;

        public KafkaConsumerService(
            IConfiguration configuration,
            ILogger<KafkaConsumerService> logger,
            WebSocketNotificationService notificationService)
        {
            _configuration = configuration;
            _logger = logger;
            _notificationService = notificationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "scanflow-backend-consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            var topic = _configuration["Kafka:ScanResultTopic"];

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);

            _logger.LogInformation("Kafka Consumer is started. Topic: {Topic}", topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    if (result?.Message?.Value == null)
                        continue;

                    var scanResult = JsonSerializer.Deserialize<ScanResultEvent>(
                        result.Message.Value,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                    if (scanResult != null)
                    {
                        await ProcessScanResultAsync(scanResult, stoppingToken);
                        consumer.Commit(result);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Kafka tüketim hatası");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Kafka işleme hatası");
                }
            }

            consumer.Close();
        }

        private async Task ProcessScanResultAsync(
            ScanResultEvent scanResult,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Scan update event received. ScanId: {ScanId}, Status: {Status}",
                scanResult.ScanId,
                scanResult.Status);

            await _notificationService.BroadcastAsync(new
            {
                type = "scan_updated",
                scanId = scanResult.ScanId,
                status = scanResult.Status,
                assetId = scanResult.AssetId,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
