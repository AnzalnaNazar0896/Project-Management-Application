using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Constants;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ProjectManager},{AppRoles.Member}")]
    public class CommentsController : Controller
    {
        private readonly CommentService _commentService;

        public CommentsController(CommentService commentService)
        {
            _commentService = commentService;
        }

        public IActionResult Index()
        {
            return View(_commentService.GetAll().Select(c => c.ToDto()).ToList());
        }
    }
}
