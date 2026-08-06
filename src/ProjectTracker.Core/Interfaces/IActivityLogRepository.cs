using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Interfaces
{
    public interface IActivityLogRepository
    {
        List<ActivityLog> GetAll();
        List<ActivityLog> GetByProjectId(int projectId, int take = 50);
        List<ActivityLog> GetRecent(int take);
        ActivityLog? GetById(int id);
        void Add(ActivityLog log);
        void Delete(int id);
        bool Exists(int id);
        int Count();
    }
}
