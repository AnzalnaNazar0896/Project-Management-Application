namespace ProjectTracker.Models.Models.Entities
{
    public class Sprint : BaseEntity
    { 
        public string SprintName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public List<Tasks> Tasks { get; set; } = new List<Tasks>();
        public SprintStatus Status { get; set; }
    }
}
