using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.DTOs.Project;
using ProjectTracker.Services;
using ProjectTracker.Web.Services;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager},{AppRoles.Member}")]
    public class ProjectsController : Controller
    {
        private readonly ProjectService _projectService;
        private readonly EmployeeService _employeeService;
        private readonly CurrentUserService _currentUser;
        private readonly ProjectAccessService _access;

        public ProjectsController(
            ProjectService projectService,
            EmployeeService employeeService,
            CurrentUserService currentUser,
            ProjectAccessService access)
        {
            _projectService = projectService;
            _employeeService = employeeService;
            _currentUser = currentUser;
            _access = access;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            var projects = _projectService.GetProjectsForUser(roles.Contains(AppRoles.Admin), employeeId)
                .Select(p => new ProjectSummaryDTO
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    Status = p.Status,
                    Progress = p.Progress,
                    IsCompleted = p.IsCompleted,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                }).ToList();
            return View(projects);
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Employees = _employeeService.GetAll();
            return View(new CreateProjectDTO());
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProjectDTO model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Employees = _employeeService.GetAll();
                return View(model);
            }

            var userName = await _currentUser.GetDisplayNameAsync();
            var projectId = _projectService.CreateProject(model, userName);
            return RedirectToAction(nameof(Details), new { id = projectId });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanViewProject(roles, id, employeeId))
                return Forbid();

            var model = _projectService.GetProjectDetails(id);
            if (model == null)
                return NotFound();

            ViewBag.CanManage = _access.CanManageProject(roles, id, employeeId);
            ViewBag.CanEditProject = _access.CanEditProjectInfo(roles, id, employeeId);
            return View(model);
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanEditProjectInfo(roles, id, employeeId))
                return Forbid();

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
                Status = project.Status ?? "Active",
                Progress = project.Progress,
                IsCompleted = project.IsCompleted
            };
            return View(model);
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProjectDTO model)
        {
            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanEditProjectInfo(roles, model.Id, employeeId))
                return Forbid();

            if (!ModelState.IsValid)
                return View(model);

            _projectService.UpdateProject(model);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpGet]
        public async Task<IActionResult> ManageMembers(int id)
        {
            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanManageProject(roles, id, employeeId))
                return Forbid();

            var model = _projectService.GetManageMembersModel(id);
            if (model == null)
                return NotFound();
            return View(model);
        }

        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageMembers(ManageProjectMembersDTO model)
        {
            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanManageProject(roles, model.ProjectId, employeeId))
                return Forbid();

            var actor = await _currentUser.GetDisplayNameAsync() ?? "System";
            _projectService.UpdateProjectMembers(model.ProjectId, model.SelectedMemberIds, actor);
            return RedirectToAction(nameof(Details), new { id = model.ProjectId });
        }

        [HttpGet]
        public async Task<IActionResult> Info(int id) => await Details(id);

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
