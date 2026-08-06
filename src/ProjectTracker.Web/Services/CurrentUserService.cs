using Microsoft.AspNetCore.Identity;
using ProjectTracker.Core.Services;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Web.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmployeeService _employeeService;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager,
            EmployeeService employeeService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _employeeService = employeeService;
        }

        public async Task<ApplicationUser?> GetUserAsync()
        {
            var principal = _httpContextAccessor.HttpContext?.User;
            if (principal == null)
                return null;
            return await _userManager.GetUserAsync(principal);
        }

        public async Task<IList<string>> GetRolesAsync()
        {
            var user = await GetUserAsync();
            if (user == null)
                return Array.Empty<string>();
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<int?> GetEmployeeIdAsync()
        {
            var user = await GetUserAsync();
            if (user?.EmployeeId != null)
                return user.EmployeeId;

            if (user == null)
                return null;

            var employee = _employeeService.GetByUserId(user.Id);
            return employee?.Id;
        }

        public async Task<string?> GetDisplayNameAsync()
        {
            var user = await GetUserAsync();
            return user?.FullName ?? user?.Email;
        }
    }
}
