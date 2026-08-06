using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;
using ProjectTracker.Web.Services;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager},{AppRoles.Member}")]
    public class AttachmentsController : Controller
    {
        private readonly AttachmentService _attachmentService;
        private readonly CurrentUserService _currentUser;
        private readonly ProjectAccessService _access;

        public AttachmentsController(
            AttachmentService attachmentService,
            CurrentUserService currentUser,
            ProjectAccessService access)
        {
            _attachmentService = attachmentService;
            _currentUser = currentUser;
            _access = access;
        }

        public IActionResult Index()
        {
            return View(_attachmentService.GetAll().Select(a => a.ToDto()).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager}")]
        public async Task<IActionResult> UploadToProject(int projectId, IFormFile file)
        {
            var roles = await _currentUser.GetRolesAsync();
            var employeeId = await _currentUser.GetEmployeeIdAsync();
            if (!_access.CanManageProject(roles, projectId, employeeId))
                return Forbid();

            if (file == null || file.Length == 0)
                return RedirectToAction("Details", "Projects", new { id = projectId });

            var uploadRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var uploadedBy = await _currentUser.GetDisplayNameAsync() ?? "User";
            await _attachmentService.SaveFileAsync(file, uploadRoot, uploadedBy, null, projectId);
            return RedirectToAction("Details", "Projects", new { id = projectId });
        }
    }
}
