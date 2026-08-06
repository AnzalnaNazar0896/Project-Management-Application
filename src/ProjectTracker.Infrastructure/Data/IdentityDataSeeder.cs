using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Infrastructure.Repositories;
using ProjectTracker.Interfaces;
using ProjectTracker.Models.Constants;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Infrastructure.Data
{
    public static class IdentityDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in AppRoles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            const string adminEmail = "admin@projecttracker.local";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var employeeRepo = scope.ServiceProvider.GetRequiredService<IEmployeeRepository>();
                var employee = new Employee
                {
                    FirstName = "System",
                    LastName = "Admin",
                    Email = adminEmail,
                    Department = "IT",
                    Availability = "Available",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };
                employeeRepo.Add(employee);

                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = employee.FullName,
                    EmployeeId = employee.Id
                };

                var createResult = await userManager.CreateAsync(adminUser, "Admin@12345");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
                    employee.UserId = adminUser.Id;
                    employeeRepo.Update(employee);
                }
            }

            await SeedDemoDataAsync(context, scope.ServiceProvider);
        }

        private static async Task SeedDemoDataAsync(ApplicationDbContext context, IServiceProvider sp)
        {
            if (await context.Projects.AnyAsync())
                return;

            var employeeRepo = sp.GetRequiredService<IEmployeeRepository>();
            var employees = new List<Employee>
            {
                new() { FirstName = "Jane", LastName = "Manager", Email = "pm@projecttracker.local", Department = "PMO", Availability = "Available", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new() { FirstName = "John", LastName = "Member", Email = "member@projecttracker.local", Department = "Engineering", Availability = "Available", CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now }
            };
            foreach (var e in employees)
                employeeRepo.Add(e);

            var project = new Project
            {
                ProjectName = "Website Redesign",
                Description = "Corporate website modernization",
                StartDate = DateTime.Today.AddDays(-14),
                EndDate = DateTime.Today.AddDays(30),
                Status = "Active",
                Progress = 0,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };
            context.Projects.Add(project);
            await context.SaveChangesAsync();

            context.ProjectMembers.AddRange(
                new ProjectMember { ProjectId = project.Id, EmployeeId = employees[0].Id, Role = AppRoles.ProjectManager, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now },
                new ProjectMember { ProjectId = project.Id, EmployeeId = employees[1].Id, Role = AppRoles.Member, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now });

            var board = new Board { BoardName = "Main Board", ProjectId = project.Id, CreatedDate = DateTime.Now, UpdatedDate = DateTime.Now };
            context.Boards.Add(board);
            await context.SaveChangesAsync();

            var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
            foreach (var emp in employees)
            {
                var user = new ApplicationUser
                {
                    UserName = emp.Email,
                    Email = emp.Email,
                    EmailConfirmed = true,
                    FullName = emp.FullName,
                    EmployeeId = emp.Id
                };
                await userManager.CreateAsync(user, "Member@12345");
                await userManager.AddToRoleAsync(user, emp == employees[0] ? AppRoles.ProjectManager : AppRoles.Member);
                emp.UserId = user.Id;
                context.Employees.Update(emp);
            }

            await context.SaveChangesAsync();
        }
    }
}
