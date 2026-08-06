using ProjectTracker.Models.Models.Entities;
using ProjectTask = ProjectTracker.Models.Models.Entities.Tasks;

namespace ProjectTracker.Interfaces
{
    public interface IProjectRepository
    {
        List<Project> GetAll();
        List<Project> GetByIds(IEnumerable<int> projectIds);
        Project? GetById(int id);
        List<ProjectTask> GetTasksByProjectId(int projectId);
        void Add(Project project);
        void Update(Project project);
        void Delete(int id);
        bool Exists(int id);
        int Count();
        int ActiveCount();
        int CompletedCount();
    }
}
