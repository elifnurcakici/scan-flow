using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.DTOs.Assets;
using backend.Entities;
using backend.Services.Interfaces;

namespace backend.Services;

public class AssetService : IAssetService
{
    private readonly AppDbContext _context;
    public AssetService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AssetResponse> CreateAsync(long userId, CreateAssetRequest request)
    {
        var asset = new Asset
        {
            Name = request.Name.Trim(),
            Domain = request.Domain.Trim().ToLower(),
            Type = request.Type,
            UserId = userId
        };

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        return Map(asset);
    }

    public async Task<List<AssetResponse>> GetAllByUserIdAsync(long userId)
    {
        return await _context.Assets
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => Map(x))
            .ToListAsync();
    }

    public async Task<AssetResponse?> GetByIdAsync(long assetId, long userId)
    {
        var asset = await _context.Assets
            .FirstOrDefaultAsync(x => x.Id == assetId && x.UserId == userId);

        return asset is null ? null : Map(asset);

    }

    public async Task<AssetResponse?> UpdateAsync(long assetId, long userId, UpdateAssetRequest request)
    {
        var asset = await _context.Assets
            .FirstOrDefaultAsync(x => x.Id == assetId && x.UserId == userId);

        if (asset is null)
            return null;

        asset.Name = request.Name.Trim();
        asset.Domain = request.Domain.Trim().ToLower();
        asset.Type = request.Type;
        asset.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Map(asset);

    }

    public async Task<bool> DeleteAsync(long assetId, long userId)
    {
        var asset = await _context.Assets
            .FirstOrDefaultAsync(x => x.Id == assetId && x.UserId == userId);

        if (asset is null)
            return false;

        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();

        return true;
    }

    private static AssetResponse Map(Asset asset)
    {
        return new AssetResponse
        {
            Id = asset.Id,
            Name = asset.Name,
            Domain = asset.Domain,
            Type = asset.Type,
            CreatedAt = asset.CreatedAt,
            UpdatedAt = asset.UpdatedAt
        };
    }
}
