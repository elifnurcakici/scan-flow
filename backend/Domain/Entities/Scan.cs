namespace backend.Entities;

public enum ScanStatus
{
    Pending = 1,
    Running = 2,
    Finished = 3,
    Failed = 4
}

public class Scan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid AssetId { get; set; }
    public Asset Asset { get; set; } = null!;
    public Guid ScannerId { get; set; }
    public Scanner Scanner { get; set; } = null!;
    public ScanStatus Status { get; set; } = ScanStatus.Pending;
    public string? ErrorReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Vulnerability> Vulnerabilities { get; set; } = new List<Vulnerability>();
    public ICollection<ScanHistory> ScanHistories { get; set; } = new List<ScanHistory>();
}
