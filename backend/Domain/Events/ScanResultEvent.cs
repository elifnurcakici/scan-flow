namespace backend.Events
{
    public class ScanResultEvent
    {
        public Guid ScanId { get; set; }
        public Guid AssetId { get; set; }
        public Guid ScannerId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<VulnerabilityDto>? Vulnerabilities { get; set; }
        public string? ErrorReason { get; set; }
    }

    public class VulnerabilityDto
    {
        public string Type { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? CweId { get; set; }
        public decimal? CvssScore { get; set; }
        public string? CvssVector { get; set; }
        public string? Recommendation { get; set; }
    }
}
