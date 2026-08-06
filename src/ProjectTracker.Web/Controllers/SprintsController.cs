using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.DTOs.Sprint;
using ProjectTracker.Models.Models.Entities;
using ProjectTracker.Web.Services;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager},{AppRoles.Member}")]
    public class SprintsController : Controller
    {
        private readonly SprintService _sprintService;
        private readonly CurrentUserService _currentUser;
        private readonly ProjectAccessService _access;

        public SprintsController(
            SprintService sprintService,
            CurrentUserService currentUser,
            ProjectAccessService access)
        {
            _sprintService = sprintService;
            _currentUser = currentUser;
            _access = access;
        }

        public IActionResult Index(int? projectId)
        {
            if (projectId.HasValue)
            {
                ViewBag.ProjectId = projectId;
                return View("ProjectSprints", _sprintService.GetProjectSprints(projectId.Value));
            }

            return View(_sprintService.GetAllSummaries());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = _sprintService.GetSprintDetails(id);
            if (model == null)
                return NotFound();

            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanViewProject(roles, model.ProjectId, employeeId))
                return Forbid();

            return View(model);
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpGet]
        public IActionResult Create(int projectId)
        {
            return View(new CreateSprintDTO
            {
                ProjectId = projectId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(14),
                Status = SprintStatus.Planned
            });
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateSprintDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var sprintId = _sprintService.CreateSprint(model);
            return RedirectToAction(nameof(Details), new { id = sprintId });
        }
    }
}
