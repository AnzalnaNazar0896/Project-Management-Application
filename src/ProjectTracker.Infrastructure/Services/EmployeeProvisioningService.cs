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

        public async Task<EmployeeProvisioningResult> CreateEmployeeWithUserAsync(CreateEmployeeUserDTO model,string actorDisplayName)
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

        public async Task<EditEmployeeResult> UpdateEmployeeAsync(EditEmployeeUserDTO model,string actorDisplayName)
        {
            var employee = _employeeRepository.GetById(model.EmployeeId);
            if (employee == null)
            {
                return new EditEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "Team member was not found."
                };
            }

            var email = model.Email.Trim();

            if (!AppRoles.All.Contains(model.Role))
            {
                return new EditEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "Invalid application role selected."
                };
            }

            if (_employeeRepository.GetAll().Any(e =>
                    e.Id != employee.Id &&
                    e.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
            {
                return new EditEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "Another employee already uses this email."
                };
            }

            ApplicationUser? user = null;

            if (!string.IsNullOrWhiteSpace(employee.UserId))
            {
                user = await _userManager.FindByIdAsync(employee.UserId);
            }

            user ??= await _userManager.Users
                .FirstOrDefaultAsync(x => x.EmployeeId == employee.Id);

            if (user != null)
            {
                var emailUser = await _userManager.FindByEmailAsync(email);
                if (emailUser != null && emailUser.Id != user.Id)
                {
                    return new EditEmployeeResult
                    {
                        Success = false,
                        ErrorMessage = "A login account with this email already exists."
                    };
                }
            }
            else if (await _userManager.FindByEmailAsync(email) != null)
            {
                return new EditEmployeeResult
                {
                    Success = false,
                    ErrorMessage = "A login account with this email already exists."
                };
            }

            var now = DateTime.Now;
            employee.FirstName = model.FirstName.Trim();
            employee.LastName = model.LastName.Trim();
            employee.Email = email;
            employee.Department = model.Department?.Trim();
            employee.Availability = string.IsNullOrWhiteSpace(model.Availability)
                ? "Available"
                : model.Availability.Trim();
            employee.UpdatedDate = now;

            _employeeRepository.Update(employee);

            if (user != null)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, email);
                if (!setEmailResult.Succeeded)
                {
                    return new EditEmployeeResult
                    {
                        Success = false,
                        ErrorMessage = string.Join(" ", setEmailResult.Errors.Select(e => e.Description))
                    };
                }

                var setUserNameResult = await _userManager.SetUserNameAsync(user, email);
                if (!setUserNameResult.Succeeded)
                {
                    return new EditEmployeeResult
                    {
                        Success = false,
                        ErrorMessage = string.Join(" ", setUserNameResult.Errors.Select(e => e.Description))
                    };
                }

                user.FullName = employee.FullName;
                user.EmployeeId = employee.Id;

                var updateUserResult = await _userManager.UpdateAsync(user);
                if (!updateUserResult.Succeeded)
                {
                    return new EditEmployeeResult
                    {
                        Success = false,
                        ErrorMessage = string.Join(" ", updateUserResult.Errors.Select(e => e.Description))
                    };
                }

                var existingRoles = await _userManager.GetRolesAsync(user);
                if (existingRoles.Any())
                {
                    var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
                    if (!removeRolesResult.Succeeded)
                    {
                        return new EditEmployeeResult
                        {
                            Success = false,
                            ErrorMessage = string.Join(" ", removeRolesResult.Errors.Select(e => e.Description))
                        };
                    }
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, model.Role);
                if (!addRoleResult.Succeeded)
                {
                    return new EditEmployeeResult
                    {
                        Success = false,
                        ErrorMessage = string.Join(" ", addRoleResult.Errors.Select(e => e.Description))
                    };
                }

                employee.UserId = user.Id;
                _employeeRepository.Update(employee);
            }

            var desiredProjectIds = model.ProjectIds
                .Distinct()
                .Where(_projectRepository.Exists)
                .ToHashSet();

            var currentMemberships = _projectMemberRepository
                .GetByEmployeeId(employee.Id);

            foreach (var membership in currentMemberships)
            {
                if (!desiredProjectIds.Contains(membership.ProjectId))
                {
                    _projectMemberRepository.Delete(membership.Id);
                }
            }

            var projectMemberRole = model.Role == AppRoles.ProjectManager
                ? AppRoles.ProjectManager
                : AppRoles.Member;

            foreach (var projectId in desiredProjectIds)
            {
                var membership = currentMemberships
                    .FirstOrDefault(x => x.ProjectId == projectId);

                if (membership == null)
                {
                    _projectMemberRepository.Add(new ProjectMember
                    {
                        ProjectId = projectId,
                        EmployeeId = employee.Id,
                        Role = projectMemberRole,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

                    var project = _projectRepository.GetById(projectId);
                    if (project != null && !string.IsNullOrWhiteSpace(employee.Email))
                    {
                        _notificationService.NotifyMemberAdded(
                            project.ProjectName,
                            employee.Email,
                            actorDisplayName);
                    }
                }
                else if (membership.Role != projectMemberRole)
                {
                    membership.Role = projectMemberRole;
                    membership.UpdatedDate = now;
                    _projectMemberRepository.Update(membership);
                }
            }

            return new EditEmployeeResult { Success = true };
        }

        public async Task<EditEmployeeUserDTO?> GetEmployeeForEditAsync(int employeeId)
        {
            var employee = _employeeRepository.GetById(employeeId);
            if (employee == null)
                return null;

            var user = !string.IsNullOrWhiteSpace(employee.UserId)
                ? await _userManager.FindByIdAsync(employee.UserId)
                : await _userManager.Users.FirstOrDefaultAsync(x => x.EmployeeId == employee.Id);

            var role = AppRoles.Member;
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);
                role = roles.FirstOrDefault() ?? AppRoles.Member;
            }

            var projectIds = _projectMemberRepository
                .GetByEmployeeId(employee.Id)
                .Select(x => x.ProjectId)
                .ToList();

            return new EditEmployeeUserDTO
            {
                EmployeeId = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = user?.Email ?? employee.Email,
                Department = employee.Department,
                Availability = employee.Availability,
                Role = role,
                ProjectIds = projectIds,
                HasLogin = user != null
            };
        }

        public async Task<List<TeamMemberListItemDTO>> GetTeamMembersAsync()
        {
            var employees = _employeeRepository.GetAll();

            var users = await _userManager.Users
                .Where(u => u.EmployeeId != null)
                .ToListAsync();

            var userByEmployeeId = users
                .Where(u => u.EmployeeId.HasValue)
                .ToDictionary(u => u.EmployeeId!.Value, u => u);

           
            var members = new List<TeamMemberListItemDTO>();

            foreach (var employee in employees)
            {
                userByEmployeeId.TryGetValue(employee.Id, out var user);

                string? role = null;

                if (user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    role = roles.FirstOrDefault();
                }
                var projectMemberships =
                            _projectMemberRepository.GetByEmployeeId(employee.Id);

                var projects = projectMemberships
                    .Select(pm => _projectRepository.GetById(pm.ProjectId))
                    .Where(p => p != null)
                    .Select(p => p!.ProjectName)
                    .OrderBy(x => x)
                    .ToList();

                members.Add(new TeamMemberListItemDTO
                {
                    EmployeeId = employee.Id,
                    Name = employee.FullName,
                    Email = employee.Email,
                    Department = employee.Department,
                    Availability = employee.Availability,
                    HasLogin = user != null,
                    ApplicationRole = role,

                    ProjectCount = projects.Count,

                    Projects = projects
                });
            }

            return members;
        }
    }
}
