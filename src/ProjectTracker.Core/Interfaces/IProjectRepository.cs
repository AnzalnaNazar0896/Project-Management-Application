using ProjectTracker.Models.Models.Entities;
using ProjectTask = ProjectTracker.Models.Models.Entities.Tasks;

namespace ProjectTracker.Interfaces
{
    public interface IProjectRepository
    {
       public List<Project> GetAll();
       public Project GetById(int id);
       public List<ProjectTask> GetTasksByProjectId(int projectId);
       public void Add(Project project);
       public void Update(Project project);
       public void Delete(int id);
       public int Count();
       public int ActiveCount();
    }
}
