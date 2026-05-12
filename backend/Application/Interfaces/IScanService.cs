using backend.DTOs.Scans;

namespace backend.Services.Interfaces;

public interface IScanService
{
    Task<Guid> StartScanAsync(Guid userId, CreateScanRequest request);
}
