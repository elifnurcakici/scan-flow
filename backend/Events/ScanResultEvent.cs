namespace backend.Events
{
    public class ScanResultEvent
    {
        public long ScanId { get; set; }
        public long AssetId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<VulnerabilityDto>? Vulnerabilities { get; set; }
        public string? ErrorReason { get; set; }
    }

    public class VulnerabilityDto
    {
        public string Type { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
