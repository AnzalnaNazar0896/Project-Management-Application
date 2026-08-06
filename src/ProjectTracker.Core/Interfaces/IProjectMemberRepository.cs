using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Interfaces
{
    public interface IProjectMemberRepository
    {
        List<ProjectMember> GetAll();
        List<ProjectMember> GetByProjectId(int projectId);
        List<ProjectMember> GetByEmployeeId(int employeeId);
        ProjectMember? GetById(int id);
        void Add(ProjectMember member);
        void Update(ProjectMember member);
        void Delete(int id);
        bool Exists(int id);
        int Count();
        bool IsMember(int projectId, int employeeId);
    }
}
