using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Models.DTOs.Project;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Services
{
    public partial class ProjectService
    {
        public ManageProjectMembersDTO? GetManageMembersModel(int projectId)
        {
            var project = _repository.GetById(projectId);
            if (project == null)
                return null;

            var members = _memberRepository.GetByProjectId(projectId);
            var employees = _employeeRepository.GetAll();

            return new ManageProjectMembersDTO
            {
                ProjectId = projectId,
                ProjectName = project.ProjectName,
                SelectedMemberIds = members.Select(m => m.EmployeeId).ToList(),
                AvailableEmployees = employees.Select(e => new EmployeeListItemDTO
                {
                    Id = e.Id,
                    Name = e.FullName,
                    Email = e.Email
                }).ToList(),
                CurrentMembers = members.Select(m => new ProjectMemberSummaryDTO
                {
                    EmployeeId = m.EmployeeId,
                    Name = m.Employee?.FullName ?? "",
                    Role = m.Role,
                    Availability = m.Employee?.Availability ?? "Available"
                }).ToList()
            };
        }

        public void UpdateProjectMembers(int projectId, IEnumerable<int> memberIds, string actorUserName)
        {
            var project = _repository.GetById(projectId);
            if (project == null)
                return;

            var existing = _memberRepository.GetByProjectId(projectId);
            var desired = memberIds.Distinct().ToHashSet();

            foreach (var member in existing.Where(m => !desired.Contains(m.EmployeeId)))
            {
                _memberRepository.Delete(member.Id);
            }

            foreach (var employeeId in desired)
            {
                if (_memberRepository.IsMember(projectId, employeeId))
                    continue;

                if (!_employeeRepository.Exists(employeeId))
                    continue;

                _memberRepository.Add(new ProjectMember
                {
                    ProjectId = projectId,
                    EmployeeId = employeeId,
                    Role = AppRoles.Member,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                });

                var employee = _employeeRepository.GetById(employeeId);
                if (employee != null)
                {
                    _notificationService.NotifyMemberAdded(project.ProjectName, employee.Email, actorUserName);
                }
            }
        }
    }
}
