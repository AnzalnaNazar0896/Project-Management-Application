namespace ProjectTracker.Models.Models.Entities
{
    public class Notification : BaseEntity
    {
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public string Receiver { get; set; }
        public bool IsRead { get; set; }
    }
}
