using ProjectTracker.Core.Interfaces;
using ProjectTracker.Interfaces;
using ProjectTracker.Models.Models.DTOs.Search;

namespace ProjectTracker.Core.Services
{
    public class SearchService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly ISprintRepository _sprintRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public SearchService(
            IProjectRepository projectRepository,
            ITaskRepository taskRepository,
            IBoardRepository boardRepository,
            ISprintRepository sprintRepository,
            IEmployeeRepository employeeRepository)
        {
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _boardRepository = boardRepository;
            _sprintRepository = sprintRepository;
            _employeeRepository = employeeRepository;
        }

        public SearchResultsDTO Search(string? query)
        {
            var results = new SearchResultsDTO { Query = query ?? "" };
            if (string.IsNullOrWhiteSpace(query))
                return results;

            var q = query.Trim();

            results.Projects = _projectRepository.GetAll()
                .Where(p => p.ProjectName.Contains(q, StringComparison.OrdinalIgnoreCase)
                    || p.Description.Contains(q, StringComparison.OrdinalIgnoreCase))
                .Select(p => new SearchItemDTO
                {
                    Title = p.ProjectName,
                    Subtitle = p.Status ?? "",
                    Url = $"/Projects/Details/{p.Id}"
                }).ToList();

            results.Tasks = _taskRepository.GetAll()
                .Where(t => t.Title.Contains(q, StringComparison.OrdinalIgnoreCase)
                    || t.Description.Contains(q, StringComparison.OrdinalIgnoreCase))
                .Select(t => new SearchItemDTO
                {
                    Title = t.Title,
                    Subtitle = t.Board?.Project?.ProjectName ?? "",
                    Url = $"/Tasks/Details/{t.Id}"
                }).ToList();

            results.Boards = _boardRepository.GetAll()
                .Where(b => b.BoardName.Contains(q, StringComparison.OrdinalIgnoreCase))
                .Select(b => new SearchItemDTO
                {
                    Title = b.BoardName,
                    Subtitle = b.Project?.ProjectName ?? "",
                    Url = $"/Boards/Kanban/{b.Id}"
                }).ToList();

            results.Sprints = _sprintRepository.GetAll()
                .Where(s => s.SprintName.Contains(q, StringComparison.OrdinalIgnoreCase))
                .Select(s => new SearchItemDTO
                {
                    Title = s.SprintName,
                    Subtitle = s.Project?.ProjectName ?? "",
                    Url = $"/Sprints/Details/{s.Id}"
                }).ToList();

            results.Employees = _employeeRepository.GetAll()
                .Where(e => e.FullName.Contains(q, StringComparison.OrdinalIgnoreCase)
                    || e.Email.Contains(q, StringComparison.OrdinalIgnoreCase))
                .Select(e => new SearchItemDTO
                {
                    Title = e.FullName,
                    Subtitle = e.Email,
                    Url = $"/Employees/Details/{e.Id}"
                }).ToList();

            return results;
        }
    }
}
