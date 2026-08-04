using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Models.DTOs.Sprint;

namespace ProjectTracker.Web.Controllers
{
    public class SprintsController : Controller
    {
        private readonly SprintService _sprintService;

        public SprintsController(SprintService sprintService)
        {
            _sprintService = sprintService;
        }

        public IActionResult Index(int projectId)
        {
            var sprints =
                _sprintService.GetProjectSprints(projectId);

            return View(sprints);
        }

        [HttpGet]
        public IActionResult Create(int projectId)
        {
            return View(new CreateSprintDTO
            {
                ProjectId = projectId
            });
        }

        [HttpPost]
        public IActionResult Create(CreateSprintDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _sprintService.CreateSprint(model);

            return RedirectToAction(
                nameof(Index),
                new { projectId = model.ProjectId });
        }
    }
}
