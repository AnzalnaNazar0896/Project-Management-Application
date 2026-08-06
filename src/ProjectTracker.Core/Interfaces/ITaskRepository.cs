using ProjectTracker.Models.Models.Entities;
using ProjectTask = ProjectTracker.Models.Models.Entities.Tasks;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Core.Interfaces
{
    public interface ITaskRepository
    {
        List<ProjectTask> GetAll();
        List<ProjectTask> GetByBoardId(int boardId);
        List<ProjectTask> GetByProjectId(int projectId);
        List<ProjectTask> GetByAssigneeId(int employeeId);
        List<ProjectTask> GetByStatus(TaskStatus? status);
        List<ProjectTask> GetOverdue();
        ProjectTask? GetById(int id);
        ProjectTask? GetDetails(int id);
        void Add(ProjectTask task);
        void Update(ProjectTask task);
        void Delete(int id);
        bool Exists(int id);
        int Count();
        int CountByStatus(TaskStatus status);
    }
}
