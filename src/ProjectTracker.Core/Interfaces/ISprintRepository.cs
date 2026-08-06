using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Interfaces
{
    public interface ISprintRepository
    {
        List<Sprint> GetAll();
        List<Sprint> GetByProjectId(int projectId);
        Sprint? GetById(int id);
        Sprint? GetCurrentForProject(int projectId);
        void Add(Sprint sprint);
        void Update(Sprint sprint);
        void Delete(int id);
        bool Exists(int id);
        int Count();
    }
}
