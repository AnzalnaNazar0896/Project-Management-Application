using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Core.Services;
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

        public TaskService(
            ITaskRepository repository,
            IBoardRepository boardRepository,
            IEmployeeRepository employeeRepository,
            NotificationService notificationService,
            ProjectService projectService)
        {
            _repository = repository;
            _boardRepository = boardRepository;
            _employeeRepository = employeeRepository;
            _notificationService = notificationService;
            _projectService = projectService;
        }

        public List<Tasks> GetBoardTasks(int boardId) => _repository.GetByBoardId(boardId);

        public List<TaskSummaryDTO> GetAllSummaries(TaskStatus? status = null)
        {
            var tasks = status.HasValue
                ? _repository.GetByStatus(status)
                : _repository.GetAll();

            if (status == TaskStatus.Pending)
            {
                tasks = tasks.Where(t => t.Status.IsPending()).ToList();
            }

            return tasks.Select(t => t.ToSummary()).ToList();
        }

        public TaskDetailsDTO? GetTaskDetails(int id)
        {
            var task = _repository.GetDetails(id);
            if (task == null)
                return null;

            return new TaskDetailsDTO
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

        public int CreateTask(CreateTaskDTO model)
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

            if (model.AssignedEmployeeId.HasValue)
            {
                var employee = _employeeRepository.GetById(model.AssignedEmployeeId.Value);
                if (employee != null)
                {
                    _notificationService.NotifyTaskAssigned(task.Title, employee.Email);
                }
            }

            var board = _boardRepository.GetById(model.BoardId);
            if (board != null)
                _projectService.RecalculateProgress(board.ProjectId);

            return task.Id;
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
    }
}
