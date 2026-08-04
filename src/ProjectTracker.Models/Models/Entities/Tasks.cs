using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;


namespace ProjectTracker.Models.Models.Entities
{
    public class Tasks : BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
        public int? SprintId { get; set; }
        public Sprint? Sprint { get; set; }
    }
}
