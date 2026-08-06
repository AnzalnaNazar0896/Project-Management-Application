using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;
using ProjectTracker.Web.Services;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager},{AppRoles.Member}")]
    public class NotificationsController : Controller
    {
        private readonly NotificationService _notificationService;
        private readonly CurrentUserService _currentUser;

        public NotificationsController(NotificationService notificationService, CurrentUserService currentUser)
        {
            _notificationService = notificationService;
            _currentUser = currentUser;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _currentUser.GetUserAsync();
            var roles = await _currentUser.GetRolesAsync();
            var items = roles.Contains(AppRoles.Admin)
                ? _notificationService.GetAll().Select(n => n.ToDto()).ToList()
                : _notificationService.GetByReceiver(user?.Email ?? "").Select(n => n.ToDto()).ToList();

            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkRead(int id)
        {
            _notificationService.MarkAsRead(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
