using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Services;
using ProjectTracker.Interfaces;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.DTOs.Team;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Infrastructure.Services
{
    public class EmployeeProvisioningService : IEmployeeProvisioningService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly NotificationService _notificationService;

        public EmployeeProvisioningService(
            UserManager<ApplicationUser> userManager,
            IEmployeeRepository employeeRepository,
            IProjectMemberRepository projectMemberRepository,
            IProjectRepository projectRepository,
            NotificationService notificationService)
        {
            _userManager = userManager;
            _employeeRepository = employeeRepository;
            _projectMemberRepository = projectMemberRepository;
            _projectRepository = projectRepository;
            _notificationService = notificationService;
        }

        public async Task<EmployeeProvisioningResult> CreateEmployeeWithUserAsync(
            CreateEmployeeUserDTO model,
            string actorDisplayName)
        {
            var email = model.Email.Trim();
            if (!AppRoles.All.Contains(model.Role))
            {
                return new EmployeeProvisioningResult
                {
                    Success = false,
                    ErrorMessage = "Invalid application role selected."
                };
            }

            if (await _userManager.FindByEmailAsync(email) != null)
            {
                return new EmployeeProvisioningResult
                {
                    Success = false,
                    ErrorMessage = "A login account with this email already exists."
                };
            }

            if (_employeeRepository.GetAll().Any(e =>
                    e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                return new EmployeeProvisioningResult
                {
                    Success = false,
                    ErrorMessage = "An employee with this email already exists."
                };
            }

            var now = DateTime.Now;
            var employee = new Employee
            {
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Email = email,
                Department = model.Department?.Trim(),
                Availability = string.IsNullOrWhiteSpace(model.Availability)
                    ? "Available"
                    : model.Availability.Trim(),
                CreatedDate = now,
                UpdatedDate = now
            };

            _employeeRepository.Add(employee);

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = employee.FullName,
                EmployeeId = employee.Id
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);
            if (!createResult.Succeeded)
            {
                _employeeRepository.Delete(employee.Id);
                return new EmployeeProvisioningResult
                {
                    Success = false,
                    ErrorMessage = string.Join(" ", createResult.Errors.Select(e => e.Description))
                };
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.Role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                _employeeRepository.Delete(employee.Id);
                return new EmployeeProvisioningResult
                {
                    Success = false,
                    ErrorMessage = string.Join(" ", roleResult.Errors.Select(e => e.Description))
                };
            }

            employee.UserId = user.Id;
            _employeeRepository.Update(employee);

            var projectMemberRole = model.Role == AppRoles.ProjectManager
                ? AppRoles.ProjectManager
                : AppRoles.Member;

            foreach (var projectId in model.ProjectIds.Distinct())
            {
                if (!_projectRepository.Exists(projectId))
                    continue;

                if (_projectMemberRepository.IsMember(projectId, employee.Id))
                    continue;

                _projectMemberRepository.Add(new ProjectMember
                {
                    ProjectId = projectId,
                    EmployeeId = employee.Id,
                    Role = projectMemberRole,
                    CreatedDate = now,
                    UpdatedDate = now
                });

                var project = _projectRepository.GetById(projectId);
                if (project != null)
                {
                    _notificationService.NotifyMemberAdded(
                        project.ProjectName,
                        employee.Email,
                        actorDisplayName);
                }
            }

            return new EmployeeProvisioningResult
            {
                Success = true,
                EmployeeId = employee.Id
            };
        }

        public async Task<List<TeamMemberListItemDTO>> GetTeamRosterAsync()
        {
            var employees = _employeeRepository.GetAll();
            var users = await _userManager.Users
                .Where(u => u.EmployeeId != null)
                .ToListAsync();

            var userByEmployeeId = users
                .Where(u => u.EmployeeId.HasValue)
                .ToDictionary(u => u.EmployeeId!.Value, u => u);

            var roster = new List<TeamMemberListItemDTO>();
            foreach (var employee in employees)
            {
                userByEmployeeId.TryGetValue(employee.Id, out var user);
                string? role = null;
                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    role = roles.FirstOrDefault();
                }

                var projectCount = _projectMemberRepository.GetByEmployeeId(employee.Id).Count;

                roster.Add(new TeamMemberListItemDTO
                {
                    EmployeeId = employee.Id,
                    Name = employee.FullName,
                    Email = employee.Email,
                    Department = employee.Department,
                    Availability = employee.Availability,
                    HasLogin = user != null,
                    ApplicationRole = role,
                    ProjectCount = projectCount
                });
            }

            return roster;
        }
    }
}
