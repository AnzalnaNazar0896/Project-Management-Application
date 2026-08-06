namespace ProjectTracker.Models.Models.Entities
{
    public class Comment : BaseEntity
    {
        public string Message { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public int TaskItemId { get; set; }
        public Tasks TaskItem { get; set; } = null!;
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public void UpdateComment(string newMessage)
        {
            Message = newMessage;
            UpdatedDate = DateTime.Now;
        }
    }
}
