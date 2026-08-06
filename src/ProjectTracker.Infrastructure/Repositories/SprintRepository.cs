using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;
using ProjectTracker.Models.Models.Enums;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class SprintRepository : ISprintRepository
    {
        private readonly ApplicationDbContext _context;

        public SprintRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Sprint> GetAll() =>
            _context.Sprints
                .Include(x => x.Project)
                .Include(x => x.Tasks)
                .OrderByDescending(x => x.StartDate)
                .ToList();

        public List<Sprint> GetByProjectId(int projectId) =>
            _context.Sprints
                .Where(x => x.ProjectId == projectId)
                .Include(x => x.Tasks)
                .OrderByDescending(x => x.StartDate)
                .ToList();

        public Sprint? GetById(int id) =>
            _context.Sprints
                .Include(x => x.Project)
                .Include(x => x.Tasks)
                .ThenInclude(t => t.AssignedEmployee)
                .FirstOrDefault(x => x.Id == id);

        public Sprint? GetCurrentForProject(int projectId)
        {
            var today = DateTime.Today;
            return _context.Sprints
                .Include(s => s.Project)
                .Include(s => s.Tasks)
                .Where(s => s.ProjectId == projectId
                    && s.StartDate <= today
                    && s.EndDate >= today
                    && s.Status == SprintStatus.Active)
                .OrderBy(s => s.StartDate)
                .FirstOrDefault();
        }

        public void Add(Sprint sprint)
        {
            _context.Sprints.Add(sprint);
            _context.SaveChanges();
        }

        public void Update(Sprint sprint)
        {
            _context.Sprints.Update(sprint);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var sprint = _context.Sprints.FirstOrDefault(x => x.Id == id);
            if (sprint != null)
            {
                _context.Sprints.Remove(sprint);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.Sprints.Any(x => x.Id == id);

        public int Count() => _context.Sprints.Count();
    }
}
