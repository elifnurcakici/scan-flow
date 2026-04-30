namespace backend.Entities;

public enum AssetType
{
    Domain = 1,
    Ip = 2,
    WebApp = 3
}

public class Asset
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public AssetType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public long UserId { get; set; }
    public User User { get; set; } = null!;
}