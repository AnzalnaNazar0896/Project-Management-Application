using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Models.DTOs.Task;
using ProjectTracker.Models.Models.Entities;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Web.Controllers
{
    public class TasksController : Controller
    {
        private readonly TaskService _taskService;

        public TasksController(TaskService taskService)
        {
            _taskService = taskService;
        }

        public IActionResult Index(int boardId)
        {
            var tasks = _taskService.GetBoardTasks(boardId);

            ViewBag.BoardId = boardId;

            return View(tasks);
        }

        [HttpGet]
        public IActionResult Create(int boardId)
        {
            return View(new CreateTaskDTO
            {
                BoardId = boardId,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium
            });
        }

        [HttpPost]
        public IActionResult Create(CreateTaskDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _taskService.CreateTask(model);

            return RedirectToAction(nameof(Index),new { boardId = model.BoardId });
        }
    }
}
