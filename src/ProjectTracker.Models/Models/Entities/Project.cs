namespace ProjectTracker.Models.Models.Entities
{
    public class Project : BaseEntity
    {
        public string ProjectName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }

        public ICollection<Board> Boards { get; set; } = new List<Board>();
        public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
        public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

        public Project()
        {
        }

        public Project(string projectName, string description)
        {
            ProjectName = projectName;
            Description = description;
            Status = "Active";
            Progress = 0;
            IsCompleted = false;
            CreatedDate = DateTime.Now;
            UpdatedDate = DateTime.Now;
        }

        public void CompleteProject()
        {
            IsCompleted = true;
            Status = "Completed";
            Progress = 100;
            UpdatedDate = DateTime.Now;
        }
    }
}
