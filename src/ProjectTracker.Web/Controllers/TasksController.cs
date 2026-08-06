using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.DTOs.Board;
using ProjectTracker.Models.Models.DTOs.Task;
using ProjectTracker.Models.Models.Entities;
using ProjectTracker.Web.Services;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager},{AppRoles.Member}")]
    public class TasksController : Controller
    {
        private readonly TaskService _taskService;
        private readonly BoardService _boardService;
        private readonly SprintService _sprintService;
        private readonly EmployeeService _employeeService;
        private readonly CommentService _commentService;
        private readonly AttachmentService _attachmentService;
        private readonly CurrentUserService _currentUser;
        private readonly ProjectAccessService _access;

        public TasksController(
            TaskService taskService,
            BoardService boardService,
            SprintService sprintService,
            EmployeeService employeeService,
            CommentService commentService,
            AttachmentService attachmentService,
            CurrentUserService currentUser,
            ProjectAccessService access)
        {
            _taskService = taskService;
            _boardService = boardService;
            _sprintService = sprintService;
            _employeeService = employeeService;
            _commentService = commentService;
            _attachmentService = attachmentService;
            _currentUser = currentUser;
            _access = access;
        }

        public async Task<IActionResult> Index(string? status)
        {
            TaskStatus? filter = null;
            if (string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase))
                filter = TaskStatus.Completed;
            else if (string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase))
                filter = TaskStatus.Pending;

            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            var tasks = _taskService.GetAllSummaries(filter);

            if (!roles.Contains(AppRoles.Admin))
            {
                tasks = tasks.Where(t =>
                    roles.Contains(AppRoles.ProjectManager) ||
                    t.AssignedEmployeeId == employeeId).ToList();
            }

            ViewBag.StatusFilter = status;
            return View(tasks);
        }

        public IActionResult Board(int boardId)
        {
            var tasks = _taskService.GetBoardTasks(boardId);
            ViewBag.BoardId = boardId;
            return View("BoardTasks", tasks);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = _taskService.GetTaskDetails(id);
            if (model == null)
                return NotFound();

            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanViewProject(roles, model.ProjectId, employeeId))
                return Forbid();

            ViewBag.CanUpdateStatus = _access.CanUpdateTask(roles, model.AssignedEmployeeId, employeeId)
                || _access.CanManageProject(roles, model.ProjectId, employeeId);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create(int? boardId, int? projectId)
        {
            ViewBag.Boards = boardId.HasValue && _boardService.GetById(boardId.Value) is { } board
                ? new List<BoardSummaryDTO> { board.ToSummary() }
                : _boardService.GetAllSummaries();
            ViewBag.Sprints = projectId.HasValue
                ? _sprintService.GetProjectSprints(projectId.Value)
                : new List<Sprint>();
            ViewBag.Employees = _employeeService.GetAll();

            return View(new CreateTaskDTO
            {
                BoardId = boardId ?? 0,
                ProjectId = projectId,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        public IActionResult Create(CreateTaskDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = _employeeService.GetAll();
                return View(model);
            }

            var taskId = _taskService.CreateTask(model);
            return RedirectToAction(nameof(Details), new { id = taskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, TaskStatus status)
        {
            var task = _taskService.GetTaskDetails(id);
            if (task == null)
                return NotFound();

            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanUpdateTask(roles, task.AssignedEmployeeId, employeeId)
                && !_access.CanManageProject(roles, task.ProjectId, employeeId))
                return Forbid();

            var userName = await _currentUser.GetDisplayNameAsync();
            _taskService.UpdateTaskStatus(id, status, userName);
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int taskId, string message)
        {
            var task = _taskService.GetTaskDetails(taskId);
            if (task == null)
                return NotFound();

            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanViewProject(roles, task.ProjectId, employeeId))
                return Forbid();

            var displayName = await _currentUser.GetDisplayNameAsync() ?? "User";
            _commentService.Add(taskId, message, displayName, employeeId);
            return RedirectToAction(nameof(Details), new { id = taskId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAttachment(int taskId, IFormFile file)
        {
            var task = _taskService.GetTaskDetails(taskId);
            if (task == null || file == null || file.Length == 0)
                return RedirectToAction(nameof(Details), new { id = taskId });

            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanViewProject(roles, task.ProjectId, employeeId))
                return Forbid();

            var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var uploadedBy = await _currentUser.GetDisplayNameAsync() ?? "User";
            await _attachmentService.SaveFileAsync(file, uploadRoot, uploadedBy, taskId, null);
            return RedirectToAction(nameof(Details), new { id = taskId });
        }
    }
}
