using backend.DTOs.Assets;
using backend.Entities;

namespace backend.Services.Interfaces;

public interface IAssetService
{
    Task<AssetResponse> CreateAsync(long userId, CreateAssetRequest request);
    Task<List<AssetResponse>> GetAllByUserIdAsync(long userId);
    Task<AssetResponse?> GetByIdAsync(long assetId, long userId);
    Task<AssetResponse?> UpdateAsync(long assetId, long userId, UpdateAssetRequest request);
    Task<bool> DeleteAsync(long assetId, long userId);
}
