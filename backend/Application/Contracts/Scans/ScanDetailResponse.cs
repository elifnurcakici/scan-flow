namespace backend.DTOs.Scans;

public class ScanDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string AssetDomain { get; set; } = string.Empty;
    public string ScannerName { get; set; } = string.Empty;
    public string ScannerType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ErrorReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ScanHistoryResponse> History { get; set; } = [];
    public List<VulnerabilityResponse> Vulnerabilities { get; set; } = [];
}
