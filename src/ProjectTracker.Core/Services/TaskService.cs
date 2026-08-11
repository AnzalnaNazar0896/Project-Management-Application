using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.DTOs.Task;
using ProjectTracker.Models.Models.Entities;
using ProjectTracker.Services;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Core.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _repository;
        private readonly IBoardRepository _boardRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly NotificationService _notificationService;
        private readonly ProjectService _projectService;
        private readonly ActivityService _activityService;
        private readonly ProjectAccessService _access;

        public TaskService(
            ITaskRepository repository,
            IBoardRepository boardRepository,
            IEmployeeRepository employeeRepository,
            NotificationService notificationService,
            ProjectService projectService,
            ActivityService activityService,
            ProjectAccessService access)
        {
            _repository = repository;
            _boardRepository = boardRepository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _projectService = projectService;
            _activityService = activityService;
            _access = access;
        }

        public List<Tasks> GetBoardTasks(int boardId) => _repository.GetByBoardId(boardId);

        public List<TaskSummaryDTO> GetAllSummaries(TaskStatus? status = null)
        {
            var tasks = status.HasValue
                ? _repository.GetByStatus(status)
                : _repository.GetAll();

            if (status == TaskStatus.Pending)
                tasks = tasks.Where(t => t.Status.IsPending()).ToList();

            return tasks.Select(t => t.ToSummary()).ToList();
        }

        public List<TaskSummaryDTO> GetSummariesForUser(
            IEnumerable<string> roles,
            int? employeeId,
            TaskStatus? status,
            bool myTasksOnly = false)
        {
            var summaries = GetAllSummaries(status);
            if (roles.Contains(AppRoles.Admin))
                return summaries;

            if (!employeeId.HasValue)
                return new List<TaskSummaryDTO>();

            if (myTasksOnly || (roles.Contains(AppRoles.Member) && !roles.Contains(AppRoles.ProjectManager)))
            {
                return summaries.Where(t => t.AssignedEmployeeId == employeeId).ToList();
            }

            var managedProjectIds = _access.GetManagedProjectIds(employeeId.Value);
            if (managedProjectIds.Count == 0)
                managedProjectIds = _access.GetProjectIdsForEmployee(employeeId.Value);

            return summaries.Where(t => managedProjectIds.Contains(t.ProjectId)).ToList();
        }

        public TaskDetailsDTO? GetTaskDetails(int id)
        {
            var task = _repository.GetDetails(id);
            if (task == null)
                return null;

            return MapDetails(task);
        }

        public EditTaskDTO? GetEditModel(int id)
        {
            var task = _repository.GetDetails(id);
            if (task == null)
                return null;

            return new EditTaskDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                BoardId = task.BoardId,
                SprintId = task.SprintId,
                AssignedEmployeeId = task.AssignedEmployeeId,
                ProjectId = task.Board?.ProjectId ?? 0,
                CreatedDate = task.CreatedDate
            };
        }

        public int CreateTask(CreateTaskDTO model, string? performedBy = null)
        {
            var task = new Tasks
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Priority = model.Priority,
                DueDate = model.DueDate,
                BoardId = model.BoardId,
                SprintId = model.SprintId,
                AssignedEmployeeId = model.AssignedEmployeeId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _repository.Add(task);

            var board = _boardRepository.GetById(model.BoardId);
            var projectId = board?.ProjectId;

            if (model.AssignedEmployeeId.HasValue)
            {
                var employee = _employeeRepository.GetById(model.AssignedEmployeeId.Value);
                if (employee != null)
                    _notificationService.NotifyTaskAssigned(task.Title, employee.Email);
            }

            if (board != null)
                _projectService.RecalculateProgress(board.ProjectId);

            _activityService.Log("Created", "Task", task.Id, projectId, performedBy ?? "System",
                $"Task '{task.Title}' was created.");

            return task.Id;
        }

        public void UpdateTask(EditTaskDTO model, string? performedBy = null)
        {
            var task = _repository.GetById(model.Id);
            if (task == null)
                return;

            var previousAssignee = task.AssignedEmployeeId;
            task.Title = model.Title;
            task.Description = model.Description;
            task.Status = model.Status;
            task.Priority = model.Priority;
            task.DueDate = model.DueDate;
            task.BoardId = model.BoardId;
            task.SprintId = model.SprintId;
            task.AssignedEmployeeId = model.AssignedEmployeeId;
            task.UpdatedDate = DateTime.Now;
            _repository.Update(task);

            if (model.AssignedEmployeeId.HasValue && model.AssignedEmployeeId != previousAssignee)
            {
                var employee = _employeeRepository.GetById(model.AssignedEmployeeId.Value);
                if (employee != null)
                    _notificationService.NotifyTaskAssigned(task.Title, employee.Email);
            }

            var board = _boardRepository.GetById(task.BoardId);
            if (board != null)
                _projectService.RecalculateProgress(board.ProjectId);

            _activityService.Log("Updated", "Task", task.Id, board?.ProjectId, performedBy ?? "System",
                $"Task '{task.Title}' was updated.");
        }

        public void DeleteTask(int id, string? performedBy = null)
        {
            var task = _repository.GetDetails(id);
            if (task == null)
                return;

            var projectId = task.Board?.ProjectId;
            var title = task.Title;
            var boardId = task.BoardId;

            _repository.Delete(id);

            var board = _boardRepository.GetById(boardId);
            if (board != null)
                _projectService.RecalculateProgress(board.ProjectId);

            _activityService.Log("Deleted", "Task", id, projectId, performedBy ?? "System",
                $"Task '{title}' was deleted.");
        }

        public bool MoveKanbanTask(int taskId, int boardId, TaskStatus newStatus, string? performedBy = null)
        {
            var task = _repository.GetById(taskId);
            if (task == null || task.BoardId != boardId)
                return false;

            UpdateTaskStatus(taskId, newStatus, performedBy);
            _activityService.Log("Moved", "Task", taskId, _boardRepository.GetById(boardId)?.ProjectId,
                performedBy ?? "System", $"Task moved to {newStatus} on Kanban board.");
            return true;
        }

        public void UpdateTaskStatus(int taskId, TaskStatus status, string? userName)
        {
            var task = _repository.GetById(taskId);
            if (task == null)
                return;

            var wasCompleted = task.Status.IsCompleted();
            task.Status = status;
            task.UpdatedDate = DateTime.Now;
            _repository.Update(task);

            if (!wasCompleted && status.IsCompleted() && task.AssignedEmployeeId.HasValue)
            {
                var employee = _employeeRepository.GetById(task.AssignedEmployeeId.Value);
                if (employee != null)
                    _notificationService.NotifyTaskCompleted(task.Title, employee.Email);
            }

            var board = _boardRepository.GetById(task.BoardId);
            if (board != null)
                _projectService.RecalculateProgress(board.ProjectId);
        }

        public List<Tasks> GetTasksForEmployee(int employeeId) =>
            _repository.GetByAssigneeId(employeeId);

        public int Count() => _repository.Count();

        public int CountCompleted() => _repository.CountByStatus(TaskStatus.Completed);

        public int CountPending() =>
            _repository.GetAll().Count(t => t.Status.IsPending());

        public bool Exists(int id) => _repository.Exists(id);

        private static TaskDetailsDTO MapDetails(Tasks task) => new()
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status.ToString(),
            Priority = task.Priority.ToString(),
            DueDate = task.DueDate,
            BoardId = task.BoardId,
            BoardName = task.Board?.BoardName ?? "",
            ProjectId = task.Board?.ProjectId ?? 0,
            ProjectName = task.Board?.Project?.ProjectName ?? "",
            SprintId = task.SprintId,
            SprintName = task.Sprint?.SprintName,
            AssignedEmployeeId = task.AssignedEmployeeId,
            AssignedTo = task.AssignedEmployee?.FullName,
            Comments = task.Comments.Select(c => c.ToDto()).ToList(),
            Attachments = task.Attachments.Select(a => a.ToDto()).ToList()
        };
    }
}
