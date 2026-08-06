using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Models.Models.Entities
{
    public class Tasks : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; } = null!;
        public int? SprintId { get; set; }
        public Sprint? Sprint { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public Employee? AssignedEmployee { get; set; }
        public List<Comment> Comments { get; set; } = new();
        public List<Attachment> Attachments { get; set; } = new();
    }
}
