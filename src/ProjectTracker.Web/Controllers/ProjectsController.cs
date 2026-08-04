using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.Models.Models.DTOs.Project;
using ProjectTracker.Services;

namespace ProjectTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ProjectService _projectService;

        public ProjectsController(ProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = _projectService.GetDashboard();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateProjectDTO model)
        {
            if (ModelState.IsValid)
            {
                _projectService.CreateProject(model);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Info(int id)
        {
            var project = _projectService.GetProject(id);

            if (project == null)
                return NotFound();

            var model = new EditProjectDTO
            {
                Id = project.Id,

                ProjectName = project.ProjectName,

                Description = project.Description,

                StartDate = project.StartDate,

                EndDate = project.EndDate,

                Status = project.Status,

                Progress = project.Progress,

                IsCompleted = project.IsCompleted
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Info(EditProjectDTO model)
        {
            if (ModelState.IsValid)
            {
                _projectService.UpdateProject(model);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Tasks(int id)
        {
            var project = _projectService.GetProject(id);

            if (project == null)
                return NotFound();

            var tasks = _projectService.GetTasksByProjectId(id);

            ViewBag.ProjectName = project.ProjectName;
            ViewBag.ProjectId = project.Id;

            return View(tasks);
        }

    }
}
