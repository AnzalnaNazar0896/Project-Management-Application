using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Models.Models.DTOs.Dashboard;
using ProjectTracker.Models.Models.DTOs.Sprint;
using ProjectTracker.Models.Models.DTOs.Project;
using ProjectTracker.Services;

namespace ProjectTracker.Core.Services
{
    public class DashboardService
    {
        private readonly ProjectService _projectService;
        private readonly BoardService _boardService;
        private readonly SprintService _sprintService;
        private readonly TaskService _taskService;
        private readonly NotificationService _notificationService;
        private readonly ICommentRepository _commentRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IProjectMemberRepository _projectMemberRepository;

        public DashboardService(
            ProjectService projectService,
            BoardService boardService,
            SprintService sprintService,
            TaskService taskService,
            NotificationService notificationService,
            ICommentRepository commentRepository,
            IAttachmentRepository attachmentRepository,
            IEmployeeRepository employeeRepository,
            IProjectMemberRepository projectMemberRepository)
        {
            _projectService = projectService;
            _boardService = boardService;
            _sprintService = sprintService;
            _taskService = taskService;
            _notificationService = notificationService;
            _commentRepository = commentRepository;
            _attachmentRepository = attachmentRepository;
            _employeeRepository = employeeRepository;
            _projectMemberRepository = projectMemberRepository;
        }

        public DashboardIndexDTO GetDashboard(bool isAdmin, int? employeeId)
        {
            var projects = _projectService.GetProjectsForUser(isAdmin, employeeId);
            var projectIds = projects.Select(p => p.Id).ToHashSet();

            var recentProjects = projects
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .Select(p => p.ToSummary())
                .ToList();

            var allTasks = _taskService.GetAllSummaries();
            if (!isAdmin && employeeId.HasValue)
            {
                allTasks = allTasks
                    .Where(t => projectIds.Contains(t.ProjectId)
                        || t.AssignedEmployeeId == employeeId)
                    .ToList();
            }

            var recentTasks = allTasks.Take(5).ToList();
            var upcoming = allTasks
                .Where(t => t.DueDate >= DateTime.Today)
                .OrderBy(t => t.DueDate)
                .Take(5)
                .ToList();

            var currentSprint = projectIds.Count > 0
                ? projectIds.Select(pid => _sprintService.GetCurrentForProject(pid))
                    .FirstOrDefault(s => s != null)
                : null;

            SprintSummaryDTO? currentSprintDto = null;
            if (currentSprint != null)
            {
                var tasks = currentSprint.Tasks ?? new List<Models.Models.Entities.Tasks>();
                var completed = tasks.Count(t => t.Status.IsCompleted());
                currentSprintDto = currentSprint.ToSummary(completed, tasks.Count);
            }

            var teamMembers = _projectMemberRepository.GetAll()
                .Where(m => isAdmin || !employeeId.HasValue || projectIds.Contains(m.ProjectId))
                .GroupBy(m => m.EmployeeId)
                .Select(g => g.First())
                .Take(8)
                .Select(m => new ProjectMemberSummaryDTO
                {
                    EmployeeId = m.EmployeeId,
                    Name = m.Employee?.FullName ?? "",
                    Role = m.Role,
                    Availability = m.Employee?.Availability ?? "Available"
                })
                .ToList();

            return new DashboardIndexDTO
            {
                TotalProjects = projects.Count,
                ActiveProjects = projects.Count(p => !p.IsCompleted),
                CompletedProjects = projects.Count(p => p.IsCompleted),
                TotalBoards = isAdmin ? _boardService.Count() : _boardService.GetAllSummaries().Count(b => projectIds.Contains(b.ProjectId)),
                TotalSprints = isAdmin ? _sprintService.Count() : _sprintService.GetAllSummaries().Count(s => projectIds.Contains(s.ProjectId)),
                TotalTasks = allTasks.Count,
                CompletedTasks = allTasks.Count(t => t.Status == nameof(Models.Models.Enums.TaskStatus.Completed)),
                PendingTasks = allTasks.Count(t => t.Status == nameof(Models.Models.Enums.TaskStatus.Pending) || t.Status == nameof(Models.Models.Enums.TaskStatus.Todo)),
                TotalNotifications = _notificationService.Count(),
                TotalComments = _commentRepository.Count(),
                TotalAttachments = _attachmentRepository.Count(),
                RecentProjects = recentProjects,
                RecentTasks = recentTasks,
                UpcomingDeadlines = upcoming,
                RecentNotifications = _notificationService.GetRecent(5).Select(n => n.ToDto()).ToList(),
                CurrentSprint = currentSprintDto,
                TeamMembers = teamMembers
            };
        }
    }
}
