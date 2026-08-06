namespace ProjectTracker.Models.Models.Entities
{
    public class ActivityLog : BaseEntity
    {
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public int? ProjectId { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
