using ProjectTracker.Models.Constants;

namespace ProjectTracker.Models.Models.Entities
{
    public class ProjectMember : BaseEntity
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public string Role { get; set; } = AppRoles.Member;
    }
}
