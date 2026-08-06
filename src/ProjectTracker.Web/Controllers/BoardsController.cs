using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.DTOs.Board;
using ProjectTracker.Web.Services;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager},{AppRoles.Member}")]
    public class BoardsController : Controller
    {
        private readonly BoardService _boardService;
        private readonly CurrentUserService _currentUser;
        private readonly ProjectAccessService _access;

        public BoardsController(
            BoardService boardService,
            CurrentUserService currentUser,
            ProjectAccessService access)
        {
            _boardService = boardService;
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

            return View(kanban);
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
