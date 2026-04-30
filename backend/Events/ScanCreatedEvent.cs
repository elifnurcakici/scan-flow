namespace backend.Events
{
    public class ScanCreatedEvent
    {
        public long ScanId { get; set; }
        public long AssetId { get; set; }
        public string Domain { get; set; } = string.Empty;
        public string AssetType { get; set; } = string.Empty;



    }
}