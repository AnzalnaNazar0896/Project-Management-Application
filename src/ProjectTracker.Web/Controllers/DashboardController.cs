using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models.Models.DTOs.Project;
using ProjectTracker.Services;

namespace ProjectTracker.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ProjectService _projectService;
        public DashboardController(ProjectService projectService)
        {
            _projectService = projectService;
        }
        public IActionResult Index()
        {
            var model = new ProjectsDashboardDTO()
            {
                TotalProjects = _projectService.TotalProjects(),
                ActiveProjects = _projectService.ActiveProjects()
            };

            return View(model);
        }
    }
}
