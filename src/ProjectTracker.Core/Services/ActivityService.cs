using ProjectTracker.Core.Interfaces;
using ProjectTracker.Models.Models.DTOs.Activity;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Services
{
    public class ActivityService
    {
        private readonly IActivityLogRepository _repository;

        public ActivityService(IActivityLogRepository repository)
        {
            _repository = repository;
        }

        public void Log(
            string action,
            string entityType,
            int? entityId,
            int? projectId,
            string performedBy,
            string details)
        {
            _repository.Add(new ActivityLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                ProjectId = projectId,
                PerformedBy = performedBy,
                Details = details,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            });
        }

        public List<ActivityLogDTO> GetByProject(int projectId, int take = 20) =>
            _repository.GetByProjectId(projectId, take).Select(ToDto).ToList();

        public List<ActivityLogDTO> GetRecent(int take = 20) =>
            _repository.GetRecent(take).Select(ToDto).ToList();

        private static ActivityLogDTO ToDto(ActivityLog log) => new()
        {
            Id = log.Id,
            Action = log.Action,
            EntityType = log.EntityType,
            EntityId = log.EntityId,
            ProjectId = log.ProjectId,
            PerformedBy = log.PerformedBy,
            Details = log.Details,
            CreatedDate = log.CreatedDate
        };
    }
}
