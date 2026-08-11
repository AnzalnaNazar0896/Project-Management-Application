namespace ProjectTracker.Models.Models.DTOs.Team
{
    public class TeamMemberListItemDTO
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Department { get; set; }
        public string Availability { get; set; } = string.Empty;
        public bool HasLogin { get; set; }
        public string? ApplicationRole { get; set; }
        public int ProjectCount { get; set; }
        public List<string> Projects { get; set; } = new();
    }
}
