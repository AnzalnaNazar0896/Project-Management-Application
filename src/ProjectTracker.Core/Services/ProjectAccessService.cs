using ProjectTracker.Core.Interfaces;
using ProjectTracker.Models.Constants;

namespace ProjectTracker.Core.Services
{
    public class ProjectAccessService
    {
        private readonly IProjectMemberRepository _memberRepository;

        public ProjectAccessService(IProjectMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public bool IsAdmin(IEnumerable<string> roles) =>
            roles.Contains(AppRoles.Admin);

        public bool IsProjectManager(IEnumerable<string> roles) =>
            roles.Contains(AppRoles.ProjectManager);

        public bool CanViewProject(IEnumerable<string> roles, int projectId, int? employeeId)
        {
            if (IsAdmin(roles))
                return true;

            if (!employeeId.HasValue)
                return false;

            return _memberRepository.IsMember(projectId, employeeId.Value);
        }

        public bool CanManageProject(IEnumerable<string> roles, int projectId, int? employeeId)
        {
            if (IsAdmin(roles))
                return true;

            if (!employeeId.HasValue)
                return false;

            if (IsProjectManager(roles))
            {
                var membership = _memberRepository.GetByProjectId(projectId)
                    .FirstOrDefault(m => m.EmployeeId == employeeId.Value);
                return membership != null &&
                    (membership.Role == AppRoles.ProjectManager || membership.Role == AppRoles.Admin);
            }

            return false;
        }

        public bool CanEditProjectInfo(IEnumerable<string> roles, int projectId, int? employeeId) =>
            CanManageProject(roles, projectId, employeeId);

        public bool CanUpdateTask(IEnumerable<string> roles, int? assigneeId, int? employeeId)
        {
            if (IsAdmin(roles) || IsProjectManager(roles))
                return true;

            return employeeId.HasValue && assigneeId == employeeId;
        }
    }
}
