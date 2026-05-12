using backend.Data;
using backend.DTOs.Assets;
using backend.Entities;
using backend.Exceptions;
using backend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace backend.Services;

public class AssetService : IAssetService
{
    private readonly AppDbContext _context;

    public AssetService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AssetResponse> CreateAsync(Guid userId, CreateAssetRequest request)
    {
        var normalizedDomain = NormalizeDomain(request.Domain);
        await EnsureAssetIsUniqueAsync(userId, normalizedDomain, request.Type);

        var asset = new Asset
        {
            Name = request.Name.Trim(),
            Domain = request.Domain.Trim(),
            NormalizedDomain = normalizedDomain,
            Type = request.Type,
            UserId = userId
        };

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(asset.Id, userId)
            ?? throw new InvalidOperationException("Asset could not be loaded after creation.");
    }

    public async Task<List<AssetResponse>> GetAllByUserIdAsync(Guid userId)
    {
        return await ProjectAssetResponses(_context.Assets.Where(x => x.UserId == userId))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<AssetResponse?> GetByIdAsync(Guid assetId, Guid userId)
    {
        return await ProjectAssetResponses(
                _context.Assets.Where(x => x.Id == assetId && x.UserId == userId)
            )
            .FirstOrDefaultAsync();
    }

    public async Task<AssetResponse?> UpdateAsync(Guid assetId, Guid userId, UpdateAssetRequest request)
    {
        var asset = await _context.Assets
            .FirstOrDefaultAsync(x => x.Id == assetId && x.UserId == userId);

        if (asset is null)
        {
            return null;
        }

        var normalizedDomain = NormalizeDomain(request.Domain);
        await EnsureAssetIsUniqueAsync(userId, normalizedDomain, request.Type, assetId);

        asset.Name = request.Name.Trim();
        asset.Domain = request.Domain.Trim();
        asset.NormalizedDomain = normalizedDomain;
        asset.Type = request.Type;
        asset.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return await GetByIdAsync(asset.Id, userId);
    }

    public async Task<bool> DeleteAsync(Guid assetId, Guid userId)
    {
        var asset = await _context.Assets
            .FirstOrDefaultAsync(x => x.Id == assetId && x.UserId == userId);

        if (asset is null)
        {
            return false;
        }

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task EnsureAssetIsUniqueAsync(
        Guid userId,
        string normalizedDomain,
        AssetType type,
        Guid? excludeAssetId = null)
    {
        var duplicateExists = await _context.Assets.AnyAsync(x =>
            x.UserId == userId &&
            x.NormalizedDomain == normalizedDomain &&
            x.Type == type &&
            (!excludeAssetId.HasValue || x.Id != excludeAssetId.Value)
        );

        if (duplicateExists)
        {
            throw new BadRequestException("This asset already exists for the selected type.");
        }
    }

    private static IQueryable<AssetResponse> ProjectAssetResponses(IQueryable<Asset> query)
    {
        return query.Select(asset => new AssetResponse
        {
            Id = asset.Id,
            Name = asset.Name,
            Domain = asset.Domain,
            Type = asset.Type,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt,
            ScanCount = asset.Scans.Count,
            VulnerabilityCount = asset.Scans.SelectMany(scan => scan.Vulnerabilities).Count(),
            CriticalCount = asset.Scans.SelectMany(scan => scan.Vulnerabilities)
                .Count(v => v.Severity == "Critical"),
            HighCount = asset.Scans.SelectMany(scan => scan.Vulnerabilities)
                .Count(v => v.Severity == "High")
        });
    }

    private static string NormalizeDomain(string domain)
    {
        return domain.Trim().ToLowerInvariant();
    }
}
