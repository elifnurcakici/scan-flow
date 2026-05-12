namespace backend.Entities;

public class ScanHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ScanId { get; set; }
    public Scan Scan { get; set; } = null!;
    public ScanStatus Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
