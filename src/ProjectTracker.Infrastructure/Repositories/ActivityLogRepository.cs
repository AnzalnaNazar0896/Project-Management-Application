using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly ApplicationDbContext _context;

        public ActivityLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ActivityLog> GetAll() =>
            _context.ActivityLogs.OrderByDescending(x => x.CreatedDate).ToList();

        public List<ActivityLog> GetByProjectId(int projectId, int take = 50) =>
            _context.ActivityLogs
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.CreatedDate)
                .Take(take)
                .ToList();

        public List<ActivityLog> GetRecent(int take) =>
            _context.ActivityLogs
                .OrderByDescending(x => x.CreatedDate)
                .Take(take)
                .ToList();

        public ActivityLog? GetById(int id) =>
            _context.ActivityLogs.FirstOrDefault(x => x.Id == id);

        public void Add(ActivityLog log)
        {
            _context.ActivityLogs.Add(log);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var log = GetById(id);
            if (log != null)
            {
                _context.ActivityLogs.Remove(log);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.ActivityLogs.Any(x => x.Id == id);

        public int Count() => _context.ActivityLogs.Count();
    }
}
