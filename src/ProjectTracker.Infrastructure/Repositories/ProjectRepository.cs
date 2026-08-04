using ProjectTracker.Data;
using ProjectTracker.Interfaces;
using Microsoft.EntityFrameworkCore;
using ProjectTask = ProjectTracker.Models.Models.Entities.Tasks;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly List<Project> projects = new();
        private readonly ApplicationDbContext _context;
        public ProjectRepository(
         ApplicationDbContext context)
        {
            _context = context;
        }
        public List<Project> GetAll()
        {
            return _context.Projects.OrderByDescending(x => x.CreatedDate).ToList();
        }

        public Project GetById(int id)
        {
            return _context.Projects.FirstOrDefault(x => x.Id == id);
        }

        public List<ProjectTask> GetTasksByProjectId(int projectId)
        {
            return _context.Boards
                .Where(b => b.ProjectId == projectId)
                .Include(b => b.Tasks)
                .SelectMany(b => b.Tasks)
                .OrderBy(t => t.DueDate)
                .ToList();
        }

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

        public int Count()
        {
            return _context.Projects.Count();
        }

        public int ActiveCount()
        {
            return _context.Projects.Count(x => !x.IsCompleted);
        }
    }
}
