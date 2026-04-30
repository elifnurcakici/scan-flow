namespace backend.DTOs.Scans;

public class ScanListItemResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public long AssetId { get; set; }
    public string AssetName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ErrorReason { get; set; }
    public int VulnerabilityCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
