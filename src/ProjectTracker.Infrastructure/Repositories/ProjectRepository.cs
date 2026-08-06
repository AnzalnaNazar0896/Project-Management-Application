using Microsoft.EntityFrameworkCore;
using ProjectTracker.Data;
using ProjectTracker.Interfaces;
using ProjectTracker.Models.Models.Entities;
using ProjectTask = ProjectTracker.Models.Models.Entities.Tasks;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ApplicationDbContext _context;

        public ProjectRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Project> GetAll() =>
            _context.Projects.OrderByDescending(x => x.CreatedDate).ToList();

        public List<Project> GetByIds(IEnumerable<int> projectIds) =>
            _context.Projects.Where(p => projectIds.Contains(p.Id)).ToList();

        public Project? GetById(int id) =>
            _context.Projects.FirstOrDefault(x => x.Id == id);

        public List<ProjectTask> GetTasksByProjectId(int projectId) =>
            _context.Boards
                .Where(b => b.ProjectId == projectId)
                .Include(b => b.Tasks)
                .ThenInclude(t => t.AssignedEmployee)
                .SelectMany(b => b.Tasks)
                .OrderBy(t => t.DueDate)
                .ToList();

        public void Add(Project project)
        {
            _context.Projects.Add(project);
            _context.SaveChanges();
        }

        public void Update(Project project)
        {
            _context.Projects.Update(project);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var project = GetById(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.Projects.Any(x => x.Id == id);

        public int Count() => _context.Projects.Count();

        public int ActiveCount() => _context.Projects.Count(x => !x.IsCompleted);

        public int CompletedCount() => _context.Projects.Count(x => x.IsCompleted);
    }
}
