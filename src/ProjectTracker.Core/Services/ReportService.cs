using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Interfaces;
using ProjectTracker.Models.Models.DTOs.Reports;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Core.Services
{
    public class ReportService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ISprintRepository _sprintRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ReportService(
            IProjectRepository projectRepository,
            ISprintRepository sprintRepository,
            ITaskRepository taskRepository,
            IEmployeeRepository employeeRepository)
        {
            _projectRepository = projectRepository;
            _sprintRepository = sprintRepository;
            _taskRepository = taskRepository;
            _employeeRepository = employeeRepository;
        }

        public ReportsViewDTO GetReports()
        {
            var projects = _projectRepository.GetAll();
            var sprints = _sprintRepository.GetAll();
            var tasks = _taskRepository.GetAll();

            return new ReportsViewDTO
            {
                ProjectProgress = projects.Select(p => new ChartItemDTO
                {
                    Label = p.ProjectName,
                    Value = p.Progress
                }).ToList(),
                CompletedVsPending = new List<ChartItemDTO>
                {
                    new() { Label = "Completed", Value = tasks.Count(t => t.Status.IsCompleted()) },
                    new() { Label = "Pending", Value = tasks.Count(t => !t.Status.IsCompleted()) }
                },
                MemberAvailability = _employeeRepository.GetAll().Select(e => new MemberAvailabilityDTO
                {
                    MemberName = e.FullName,
                    Availability = string.IsNullOrWhiteSpace(e.Availability) ? "Available" : e.Availability
                }).ToList(),
                OverdueTasks = _taskRepository.GetOverdue().Select(t => new TaskSummaryItemDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    ProjectName = t.Board?.Project?.ProjectName ?? "",
                    DueDate = t.DueDate,
                    AssignedTo = t.AssignedEmployee?.FullName ?? "Unassigned"
                }).ToList()
             
            };
        }
    }
}
