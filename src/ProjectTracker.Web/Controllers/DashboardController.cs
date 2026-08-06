using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;
using ProjectTracker.Web.Services;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager},{AppRoles.Member}")]
    public class DashboardController : Controller
    {
        private readonly DashboardService _dashboardService;
        private readonly CurrentUserService _currentUser;

        public DashboardController(DashboardService dashboardService, CurrentUserService currentUser)
        {
            _dashboardService = dashboardService;
            _currentUser = currentUser;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            var isAdmin = roles.Contains(AppRoles.Admin);
            var model = _dashboardService.GetDashboard(isAdmin, employeeId);
            return View(model);
        }
    }
}
