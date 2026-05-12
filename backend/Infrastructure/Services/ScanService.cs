using backend.Data;
using backend.DTOs.Scans;
using backend.Entities;
using backend.Events;
using backend.Exceptions;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class ScanService : IScanService
{
    private readonly AppDbContext _context;
    private readonly IKafkaProducerService _kafkaProducer;

    public ScanService(AppDbContext context, IKafkaProducerService kafkaProducer)
    {
        _context = context;
        _kafkaProducer = kafkaProducer;
    }

    public async Task<Guid> StartScanAsync(Guid userId, CreateScanRequest request)
    {
        var asset = await _context.Assets
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == request.AssetId && a.UserId == userId);

        if (asset is null)
        {
            throw new NotFoundException("Asset not found or access denied.");
        }

        var scanner = await _context.Scanners
            .AsNoTracking()
            .Where(x => x.AssetType == asset.Type && x.IsEnabled)
            .OrderBy(x => x.Type)
            .FirstOrDefaultAsync();

        if (scanner is null)
        {
            throw new BadRequestException("No scanner is configured for this asset type.");
        }

        var now = DateTime.UtcNow;
        var scan = new Scan
        {
            Name = request.Name.Trim(),
            AssetId = request.AssetId,
            ScannerId = scanner.Id,
            Status = ScanStatus.Pending,
            CreatedAt = now
        };

        _context.Scans.Add(scan);
        _context.ScanHistories.Add(new ScanHistory
        {
            ScanId = scan.Id,
            Status = ScanStatus.Pending,
            CreatedAt = now
        });

        await _context.SaveChangesAsync();

        await _kafkaProducer.ProducerAsync(scanner.TopicName, new ScanCreatedEvent
        {
            ScanId = scan.Id,
            AssetId = asset.Id,
            ScannerId = scanner.Id,
            ScannerName = scanner.Name,
            ScannerType = scanner.Type.ToString(),
            Domain = asset.Domain,
            AssetType = asset.Type.ToString()
        });

        return scan.Id;
    }
}
