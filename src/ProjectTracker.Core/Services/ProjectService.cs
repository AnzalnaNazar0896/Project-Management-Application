using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Core.Services;
using ProjectTracker.Interfaces;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.DTOs.Project;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Services
{
    public partial class ProjectService
    {
        private readonly IProjectRepository _repository;
        private readonly IProjectMemberRepository _memberRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly ISprintRepository _sprintRepository;
        private readonly IAttachmentRepository _attachmentRepository;
        private readonly NotificationService _notificationService;
        private readonly ActivityService _activityService;

        public ProjectService(
            IProjectRepository repository,
            IProjectMemberRepository memberRepository,
            IEmployeeRepository employeeRepository,
            ITaskRepository taskRepository,
            IBoardRepository boardRepository,
            ISprintRepository sprintRepository,
            IAttachmentRepository attachmentRepository,
            NotificationService notificationService,
            ActivityService activityService)
        {
            _repository = repository;
            _memberRepository = memberRepository;
            _employeeRepository = employeeRepository;
            _taskRepository = taskRepository;
            _boardRepository = boardRepository;
            _sprintRepository = sprintRepository;
            _attachmentRepository = attachmentRepository;
            _notificationService = notificationService;
            _activityService = activityService;
        }

        public int CreateProject(CreateProjectDTO model, string? creatorUserName = null)
        {
            var project = new Project
            {
                ProjectName = model.ProjectName,
                Description = model.Description,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = string.IsNullOrWhiteSpace(model.Status) ? "Active" : model.Status,
                Progress = 0,
                IsCompleted = false,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            _repository.Add(project);

            foreach (var employeeId in model.MemberIds.Distinct())
            {
                if (!_employeeRepository.Exists(employeeId))
                    continue;

                var employee = _employeeRepository.GetById(employeeId);
                _memberRepository.Add(new ProjectMember
                {
                    ProjectId = project.Id,
                    EmployeeId = employeeId,
                    Role = AppRoles.Member,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                });

                if (employee != null)
                {
                    _notificationService.NotifyProjectCreated(project.ProjectName, employee.Email);
                }
            }

            if (!string.IsNullOrEmpty(creatorUserName))
            {
                _notificationService.NotifyProjectCreated(project.ProjectName, creatorUserName);
            }

            _activityService.Log("Created", "Project", project.Id, project.Id, creatorUserName ?? "System",
                $"Project '{project.ProjectName}' was created.");

            return project.Id;
        }

        public List<Project> GetProjects() => _repository.GetAll();

        public List<ProjectSummaryDTO> GetProjectSummaries() =>
            _repository.GetAll().Select(p => p.ToSummary()).ToList();

        public int TotalProjects() => _repository.Count();

        public int ActiveProjects() => _repository.ActiveCount();

        public int CompletedProjects() => _repository.CompletedCount();

        public Project? GetProject(int id) => _repository.GetById(id);

        public List<Tasks> GetTasksByProjectId(int projectId) =>
            _repository.GetTasksByProjectId(projectId);

        public void UpdateProject(EditProjectDTO model)
        {
            var project = _repository.GetById(model.Id);
            if (project == null)
                return;

            project.ProjectName = model.ProjectName;
            project.Description = model.Description;
            project.StartDate = model.StartDate;
            project.EndDate = model.EndDate;
            project.Status = model.Status;
            project.Progress = model.Progress;
            project.IsCompleted = model.IsCompleted;
            project.UpdatedDate = DateTime.Now;
            _repository.Update(project);
            RecalculateProgress(model.Id);
        }

        public ProjectsDashboardDTO GetDashboard()
        {
            var projects = _repository.GetAll();
            return new ProjectsDashboardDTO
            {
                TotalProjects = projects.Count,
                ActiveProjects = projects.Count(x => !x.IsCompleted),
                CompletedProjects = projects.Count(x => x.IsCompleted),
                Projects = projects
            };
        }

        public ProjectDetailsDTO? GetProjectDetails(int id)
        {
            var project = _repository.GetById(id);
            if (project == null)
                return null;

            RecalculateProgress(id);
            project = _repository.GetById(id)!;

            var tasks = _taskRepository.GetByProjectId(id);
            var boards = _boardRepository.GetByProjectId(id);
            var sprints = _sprintRepository.GetByProjectId(id);
            var members = _memberRepository.GetByProjectId(id);
            var attachments = _attachmentRepository.GetByProjectId(id);

            var memberSummaries = members.Select(m =>
            {
                var empTasks = tasks.Where(t => t.AssignedEmployeeId == m.EmployeeId).ToList();
                return new ProjectMemberSummaryDTO
                {
                    EmployeeId = m.EmployeeId,
                    Name = m.Employee?.FullName ?? "Unknown",
                    Role = m.Role,
                    Availability = m.Employee?.Availability ?? "Available",
                    AssignedTasks = empTasks.Count,
                    CompletedTasks = empTasks.Count(t => t.Status.IsCompleted()),
                    PendingTasks = empTasks.Count(t => t.Status.IsPending() || t.Status.IsInProgress())
                };
            }).ToList();

            return new ProjectDetailsDTO
            {
                Id = project.Id,
                ProjectName = project.ProjectName,
                Description = project.Description,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Status = project.Status,
                Progress = project.Progress,
                IsCompleted = project.IsCompleted,
                Members = memberSummaries,
                Boards = boards.Select(b => b.ToSummary()).ToList(),
                Sprints = sprints.Select(s =>
                {
                    var sprintTasks = s.Tasks ?? new List<Tasks>();
                    var completed = sprintTasks.Count(t => t.Status.IsCompleted());
                    return s.ToSummary(completed, sprintTasks.Count);
                }).ToList(),
                Tasks = tasks.Select(t => t.ToSummary()).ToList(),
                Attachments = attachments.Select(a => a.ToDto()).ToList(),
                RecentNotifications = _notificationService.GetRecent(5).Select(n => n.ToDto()).ToList(),
                RecentActivity = _activityService.GetByProject(id, 15),
                TotalTasks = tasks.Count,
                CompletedTasks = tasks.Count(t => t.Status.IsCompleted()),
                PendingTasks = tasks.Count(t => t.Status.IsPending()),
                InProgressTasks = tasks.Count(t => t.Status.IsInProgress()),
                TotalSprints = sprints.Count,
                ActiveSprints = sprints.Count(s => s.Status == SprintStatus.Active),
                CompletedSprints = sprints.Count(s => s.Status == SprintStatus.Completed)
            };
        }

        public void RecalculateProgress(int projectId)
        {
            var tasks = _taskRepository.GetByProjectId(projectId);
            if (tasks.Count == 0)
                return;

            var progress = (int)Math.Round(tasks.Count(t => t.Status.IsCompleted()) * 100.0 / tasks.Count);
            var project = _repository.GetById(projectId);
            if (project == null)
                return;

            project.Progress = progress;
            project.IsCompleted = progress >= 100;
            if (project.IsCompleted)
                project.Status = "Completed";
            project.UpdatedDate = DateTime.Now;
            _repository.Update(project);
        }

        public List<Project> GetProjectsForUser(bool isAdmin, int? employeeId)
        {
            if (isAdmin || !employeeId.HasValue)
                return _repository.GetAll();

            var projectIds = _memberRepository.GetByEmployeeId(employeeId.Value)
                .Select(m => m.ProjectId)
                .Distinct()
                .ToList();
            return _repository.GetByIds(projectIds);
        }
    }
}
