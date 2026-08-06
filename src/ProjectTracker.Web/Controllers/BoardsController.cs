using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.DTOs.Board;
using ProjectTracker.Web.Services;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager},{AppRoles.Member}")]
    public class BoardsController : Controller
    {
        private readonly BoardService _boardService;
        private readonly TaskService _taskService;
        private readonly CurrentUserService _currentUser;
        private readonly ProjectAccessService _access;

        public BoardsController(
            BoardService boardService,
            TaskService taskService,
            CurrentUserService currentUser,
            ProjectAccessService access)
        {
            _boardService = boardService;
            _taskService = taskService;
            _currentUser = currentUser;
            _access = access;
        }

        public IActionResult Index(int? projectId)
        {
            var boards = projectId.HasValue
                ? _boardService.GetProjectBoards(projectId.Value).Select(b => new BoardSummaryDTO
                {
                    Id = b.Id,
                    BoardName = b.BoardName,
                    ProjectId = b.ProjectId,
                    TaskCount = b.Tasks?.Count ?? 0
                }).ToList()
                : _boardService.GetAllSummaries();

            ViewBag.ProjectId = projectId;
            return View(boards);
        }

        [HttpGet]
        public async Task<IActionResult> Kanban(int id)
        {
            var kanban = _boardService.GetKanban(id);
            if (kanban == null)
                return NotFound();

            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanViewProject(roles, kanban.ProjectId, employeeId))
                return Forbid();

            ViewBag.CanDrag = true;
            return View(kanban);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MoveTask([FromBody] KanbanMoveTaskDTO model)
        {
            var kanban = _boardService.GetKanban(model.BoardId);
            if (kanban == null)
                return NotFound();

            var task = _taskService.GetTaskDetails(model.TaskId);
            if (task == null)
                return NotFound();

            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanUpdateTask(roles, kanban.ProjectId, task.AssignedEmployeeId, employeeId))
                return Forbid();

            var userName = await _currentUser.GetDisplayNameAsync();
            var ok = _taskService.MoveKanbanTask(model.TaskId, model.BoardId, model.Status, userName);
            return ok ? Json(new { success = true }) : BadRequest();
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpGet]
        public IActionResult Create(int projectId)
        {
            return View(new CreateBoardDTO { ProjectId = projectId });
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateBoardDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var boardId = _boardService.CreateBoard(model);
            return RedirectToAction(nameof(Kanban), new { id = boardId });
        }
    }
}
