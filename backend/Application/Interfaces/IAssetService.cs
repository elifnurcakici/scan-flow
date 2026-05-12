using backend.DTOs.Assets;
using backend.Entities;

namespace backend.Services.Interfaces;

public interface IAssetService
{
    Task<AssetResponse> CreateAsync(Guid userId, CreateAssetRequest request);
    Task<List<AssetResponse>> GetAllByUserIdAsync(Guid userId);
    Task<AssetResponse?> GetByIdAsync(Guid assetId, Guid userId);
    Task<AssetResponse?> UpdateAsync(Guid assetId, Guid userId, UpdateAssetRequest request);
    Task<bool> DeleteAsync(Guid assetId, Guid userId);
}
