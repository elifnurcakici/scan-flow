using backend.DTOs.Scans;

namespace backend.Services.Interfaces;

public interface IScanService
{
    Task<List<Guid>> StartScanAsync(Guid userId, CreateScanRequest request);
}
