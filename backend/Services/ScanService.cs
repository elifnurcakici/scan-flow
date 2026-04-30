using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Scans;
using backend.Entities;
using backend.Exceptions;
using backend.Events;
using backend.Services.Interfaces;

namespace backend.Services;

public class ScanService : IScanService
{
    private readonly AppDbContext _context;
    private readonly IKafkaProducerService _kafkaProducer;
    private readonly IConfiguration _configuration;

    public ScanService(
        AppDbContext context,
        IKafkaProducerService kafkaProducer,
        IConfiguration configuration)
    {
        _context = context;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
    }

    public async Task<long> StartScanAsync(long userId, CreateScanRequest request)
    {
        var asset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == request.AssetId && a.UserId == userId);

        if (asset == null)
            throw new NotFoundException("Asset not found or access denied.");

        var scan = new Scan
        {
            Name = request.Name,
            AssetId = request.AssetId,
            Status = ScanStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Scans.Add(scan);
        await _context.SaveChangesAsync();

        var history = new ScanHistory
        {
            ScanId = scan.Id,
            Status = ScanStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.ScanHistories.Add(history);
        await _context.SaveChangesAsync();

        var scanCreatedEvent = new ScanCreatedEvent
        {
            ScanId = scan.Id,
            AssetId = asset.Id,
            Domain = asset.Domain,
            AssetType = asset.Type.ToString()
        };

        var topic = _configuration["Kafka:ScanCreatedTopic"];

        if (string.IsNullOrEmpty(topic))
            throw new InvalidOperationException("Kafka topic is not configured.");

        await _kafkaProducer.ProducerAsync(topic, scanCreatedEvent);
        return scan.Id;
    }
}
