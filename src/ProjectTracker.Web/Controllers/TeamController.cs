using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Interfaces;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.DTOs.Team;
using ProjectTracker.Web.Services;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class TeamController : Controller
    {
        private readonly IEmployeeProvisioningService _provisioning;
        private readonly IProjectRepository _projects;
        private readonly CurrentUserService _currentUser;

        public TeamController(
            IEmployeeProvisioningService provisioning,
            IProjectRepository projects,
            CurrentUserService currentUser)
        {
            _provisioning = provisioning;
            _projects = projects;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var roster = await _provisioning.GetTeamMembersAsync();
            return View(roster);
        }

        [HttpGet]
        public IActionResult Create()
        {
            PopulateProjects();
            PopulateRoles();
            return View(new CreateEmployeeUserDTO());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _provisioning.GetEmployeeForEditAsync(id);
            if (model == null)
                return NotFound();

            PopulateProjects();
            PopulateRoles();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEmployeeUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                PopulateProjects();
                PopulateRoles();
                return View(model);
            }

            var actor = await _currentUser.GetDisplayNameAsync() ?? "Admin";
            var result = await _provisioning.UpdateEmployeeAsync(model, actor);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Could not update team member.");
                PopulateProjects();
                PopulateRoles();
                return View(model);
            }

            TempData["SuccessMessage"] = "Team member updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                PopulateProjects();
                PopulateRoles();
                return View(model);
            }

            var actor = await _currentUser.GetDisplayNameAsync() ?? "Admin";
            var result = await _provisioning.CreateEmployeeWithUserAsync(model, actor);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Could not create team member.");
                PopulateProjects();
                PopulateRoles();
                return View(model);
            }

            TempData["SuccessMessage"] = "Team member created. They can sign in with the email and password you set.";
            return RedirectToAction(nameof(Index));
        }

        private void PopulateProjects()
        {
            ViewBag.Projects = _projects.GetAll()
                .OrderBy(p => p.ProjectName)
                .Select(p => new SelectListItem(p.ProjectName, p.Id.ToString()))
                .ToList();
        }

        private void PopulateRoles()
        {
            ViewBag.Roles = AppRoles.All
                .Select(r => new SelectListItem(r, r))
                .ToList();
        }
    }
}
