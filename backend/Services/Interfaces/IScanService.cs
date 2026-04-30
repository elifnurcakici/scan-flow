using backend.DTOs.Scans;

namespace backend.Services.Interfaces;

public interface IScanService
{
    Task<long> StartScanAsync(long userId, CreateScanRequest request);
}