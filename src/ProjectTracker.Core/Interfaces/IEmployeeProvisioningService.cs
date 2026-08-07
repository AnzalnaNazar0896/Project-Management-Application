using ProjectTracker.Models.Models.DTOs.Team;

namespace ProjectTracker.Core.Interfaces
{
    public class EmployeeProvisioningResult
    {
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public int? EmployeeId { get; init; }
    }

    public interface IEmployeeProvisioningService
    {
        Task<EmployeeProvisioningResult> CreateEmployeeWithUserAsync(
            CreateEmployeeUserDTO model,
            string actorDisplayName);

        Task<List<TeamMemberListItemDTO>> GetTeamRosterAsync();
    }
}
