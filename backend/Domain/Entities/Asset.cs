namespace backend.Entities;

public enum AssetType
{
    Domain = 1,
    Ip = 2,
    Image = 3,
    SourceControl = 4,
    Cluster = 5,
    Cloud = 6
}

public class Asset
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string NormalizedDomain { get; set; } = string.Empty;
    public AssetType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
}
