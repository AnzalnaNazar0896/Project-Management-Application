namespace ProjectTracker.Models.Models.Entities
{
    public class Employee : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string Availability { get; set; } = "Available";
        public string? UserId { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();

        public ICollection<ProjectMember> ProjectMembers { get; set; } = new List<ProjectMember>();
        public ICollection<Tasks> AssignedTasks { get; set; } = new List<Tasks>();
    }
}
