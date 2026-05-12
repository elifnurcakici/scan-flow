namespace backend.Entities;

public enum ScannerType
{
    Dast = 1,
    Sast = 2,
    Sca = 3,
    SecretScan = 4,
    Infra = 5,
    Cloud = 6
}

public class Scanner
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ScannerType Type { get; set; }
    public AssetType AssetType { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
}
