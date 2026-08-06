using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ProjectTracker.Core.Services;
using ProjectTracker.Web.Services;

namespace ProjectTracker.Web.Filters
{
    public class ProjectAuthorizeAttribute : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        public bool RequireManage { get; set; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User?.Identity?.IsAuthenticated != true)
                return;

            if (!context.RouteData.Values.TryGetValue("id", out var idValue) &&
                !context.RouteData.Values.TryGetValue("projectId", out idValue))
            {
                return;
            }

            if (!int.TryParse(idValue?.ToString(), out var projectId))
                return;

            var currentUser = context.HttpContext.RequestServices.GetRequiredService<CurrentUserService>();
            var access = context.HttpContext.RequestServices.GetRequiredService<ProjectAccessService>();
            var roles = await currentUser.GetRolesAsync();
            var employeeId = await currentUser.GetEmployeeIdAsync();

            var allowed = RequireManage
                ? access.CanManageProject(roles, projectId, employeeId)
                : access.CanViewProject(roles, projectId, employeeId);

            if (!allowed)
                context.Result = new ForbidResult();
        }
    }
}
