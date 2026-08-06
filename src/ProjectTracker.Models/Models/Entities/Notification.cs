namespace ProjectTracker.Models.Models.Entities
{
    public class Notification : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string NotificationType { get; set; } = string.Empty;
        public string Receiver { get; set; } = string.Empty;
        public bool IsRead { get; set; }
    }
}
