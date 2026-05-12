namespace backend.Events
{
    public class ScanCreatedEvent
    {
        public Guid ScanId { get; set; }
        public Guid AssetId { get; set; }
        public Guid ScannerId { get; set; }
        public string ScannerName { get; set; } = string.Empty;
        public string ScannerType { get; set; } = string.Empty;
        public string Domain { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;
    }
}
