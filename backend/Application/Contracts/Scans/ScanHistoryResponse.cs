namespace backend.DTOs.Scans;

public class ScanHistoryResponse
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
