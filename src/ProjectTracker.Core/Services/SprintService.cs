using ProjectTracker.Core.Interfaces;
using ProjectTracker.Core.Mapping;
using ProjectTracker.Models.Models.DTOs.Sprint;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Services
{
    public class SprintService
    {
        private readonly ISprintRepository _repository;
        private readonly IProjectMemberRepository _memberRepository;
        private readonly NotificationService _notificationService;

        public SprintService(
            ISprintRepository repository,
            IProjectMemberRepository memberRepository,
            NotificationService notificationService)
        {
            _repository = repository;
            _memberRepository = memberRepository;
            _notificationService = notificationService;
        }

        public List<Sprint> GetProjectSprints(int projectId) => _repository.GetByProjectId(projectId);

        public List<SprintSummaryDTO> GetAllSummaries()
        {
            return _repository.GetAll().Select(s =>
            {
                var tasks = s.Tasks ?? new List<Tasks>();
                var completed = tasks.Count(t => t.Status.IsCompleted());
                return s.ToSummary(completed, tasks.Count);
            }).ToList();
        }

        public SprintDetailsDTO? GetSprintDetails(int id)
        {
            var sprint = _repository.GetById(id);
            if (sprint == null)
                return null;

            var tasks = sprint.Tasks ?? new List<Tasks>();
            var completed = tasks.Count(t => t.Status.IsCompleted());
            var progress = tasks.Count == 0 ? 0 : (int)Math.Round(completed * 100.0 / tasks.Count);

            return new SprintDetailsDTO
            {
                Id = sprint.Id,
                SprintName = sprint.SprintName,
                StartDate = sprint.StartDate,
                EndDate = sprint.EndDate,
                Status = sprint.Status.ToString(),
                ProjectId = sprint.ProjectId,
                ProjectName = sprint.Project?.ProjectName ?? "",
                Progress = progress,
                Tasks = tasks.Select(t => t.ToSummary()).ToList()
            };
        }

        public int CreateSprint(CreateSprintDTO model)
        {
            if (model.EndDate < model.StartDate)
                throw new InvalidOperationException("End date cannot be before start date.");

            var sprint = new Sprint
            {
                SprintName = model.SprintName,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ProjectId = model.ProjectId,
                Status = model.Status,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _repository.Add(sprint);

            var members = _memberRepository.GetByProjectId(model.ProjectId);
            foreach (var member in members)
            {
                var email = member.Employee?.Email;
                if (!string.IsNullOrWhiteSpace(email))
                    _notificationService.NotifySprintCreated(sprint.SprintName, email);
            }

            return sprint.Id;
        }

        public int Count() => _repository.Count();

        public Sprint? GetCurrentForProject(int projectId) =>
            _repository.GetCurrentForProject(projectId);
    }
}
