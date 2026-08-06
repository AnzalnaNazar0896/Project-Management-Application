using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Services;
using ProjectTracker.Infrastructure.Email;
using ProjectTracker.Infrastructure.Repositories;
using ProjectTracker.Interfaces;
using ProjectTracker.Services;

namespace ProjectTracker.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectTrackerInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IBoardRepository, BoardRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ISprintRepository, SprintRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            services.AddScoped<IEmailNotificationService, SmtpEmailNotificationService>();

            services.AddScoped<ProjectService>();
            services.AddScoped<BoardService>();
            services.AddScoped<TaskService>();
            services.AddScoped<SprintService>();
            services.AddScoped<NotificationService>();
            services.AddScoped<DashboardService>();
            services.AddScoped<ReportService>();
            services.AddScoped<SearchService>();
            services.AddScoped<CommentService>();
            services.AddScoped<AttachmentService>();
            services.AddScoped<EmployeeService>();
            services.AddScoped<ProjectAccessService>();
            services.AddScoped<ActivityService>();

            return services;
        }
    }
}
