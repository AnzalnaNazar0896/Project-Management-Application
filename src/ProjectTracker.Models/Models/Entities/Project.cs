namespace ProjectTracker.Models.Models.Entities
{
    public class Project : BaseEntity
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public int Progress { get; set; }
        public bool IsCompleted { get; set; }

        public Project()
        {

        }

        public Project(string projectName,string description)
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
