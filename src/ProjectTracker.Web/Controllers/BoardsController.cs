using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Models.DTOs.Board;

namespace ProjectTracker.Web.Controllers
{
    public class BoardsController : Controller
    {
        private readonly BoardService _boardService;

        public BoardsController(BoardService boardService)
        {
            _boardService = boardService;
        }
        public IActionResult Index(int projectId)
        {
            var boards = _boardService.GetProjectBoards(projectId);

            ViewBag.ProjectId = projectId;

            return View(boards);
        }

        [HttpGet]
        public IActionResult Create(int projectId)
        {
            return View(new CreateBoardDTO
            {
                ProjectId = projectId
            });
        }

        [HttpPost]
        public IActionResult Create(CreateBoardDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _boardService.CreateBoard(model);

            return RedirectToAction(
                nameof(Index),
                new { projectId = model.ProjectId });
        }
    }
}
