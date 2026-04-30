namespace backend.Entities;

public class ScanHistory
{
    public long Id { get; set; }
    public long ScanId { get; set; }
    public Scan Scan { get; set; } = null!;
    public ScanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}