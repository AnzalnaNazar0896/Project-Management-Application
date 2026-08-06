namespace ProjectTracker.Models.Models.DTOs.Project
{
    public class ManageProjectMembersDTO
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public List<int> SelectedMemberIds { get; set; } = new();
        public List<EmployeeListItemDTO> AvailableEmployees { get; set; } = new();
        public List<ProjectMemberSummaryDTO> CurrentMembers { get; set; } = new();
    }

    public class EmployeeListItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
