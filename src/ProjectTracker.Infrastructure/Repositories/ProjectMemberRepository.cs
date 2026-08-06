using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectMemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ProjectMember> GetAll() =>
            _context.ProjectMembers
                .Include(m => m.Employee)
                .Include(m => m.Project)
                .ToList();

        public List<ProjectMember> GetByProjectId(int projectId) =>
            _context.ProjectMembers
                .Where(m => m.ProjectId == projectId)
                .Include(m => m.Employee)
                .ToList();

        public List<ProjectMember> GetByEmployeeId(int employeeId) =>
            _context.ProjectMembers
                .Where(m => m.EmployeeId == employeeId)
                .Include(m => m.Project)
                .ToList();

        public ProjectMember? GetById(int id) =>
            _context.ProjectMembers
                .Include(m => m.Employee)
                .FirstOrDefault(x => x.Id == id);

        public void Add(ProjectMember member)
        {
            _context.ProjectMembers.Add(member);
            _context.SaveChanges();
        }

        public void Update(ProjectMember member)
        {
            _context.ProjectMembers.Update(member);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var member = _context.ProjectMembers.FirstOrDefault(x => x.Id == id);
            if (member != null)
            {
                _context.ProjectMembers.Remove(member);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.ProjectMembers.Any(x => x.Id == id);

        public int Count() => _context.ProjectMembers.Count();

        public bool IsMember(int projectId, int employeeId) =>
            _context.ProjectMembers.Any(m => m.ProjectId == projectId && m.EmployeeId == employeeId);
    }
}
