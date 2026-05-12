using System.ComponentModel.DataAnnotations;

namespace backend.DTOs.Scans;

public class CreateScanRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid AssetId { get; set; }
}
