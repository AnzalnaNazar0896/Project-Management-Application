using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;
using ProjectTask = ProjectTracker.Models.Models.Entities.Tasks;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ProjectTask> GetAll() =>
            _context.Tasks
                .Include(t => t.Board).ThenInclude(b => b.Project)
                .Include(t => t.Sprint)
                .Include(t => t.AssignedEmployee)
                .OrderByDescending(t => t.CreatedDate)
                .ToList();

        public List<ProjectTask> GetByBoardId(int boardId) =>
            _context.Tasks
                .Where(x => x.BoardId == boardId)
                .Include(t => t.AssignedEmployee)
                .OrderBy(x => x.DueDate)
                .ToList();

        public List<ProjectTask> GetByProjectId(int projectId) =>
            _context.Tasks
                .Include(t => t.Board)
                .Where(t => t.Board.ProjectId == projectId)
                .Include(t => t.Sprint)
                .Include(t => t.AssignedEmployee)
                .ToList();

        public List<ProjectTask> GetByAssigneeId(int employeeId) =>
            _context.Tasks
                .Where(t => t.AssignedEmployeeId == employeeId)
                .Include(t => t.Board).ThenInclude(b => b.Project)
                .ToList();

        public List<ProjectTask> GetByStatus(TaskStatus? status)
        {
            var query = _context.Tasks
                .Include(t => t.Board).ThenInclude(b => b.Project)
                .Include(t => t.AssignedEmployee)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            return query.OrderBy(t => t.DueDate).ToList();
        }

        public List<ProjectTask> GetOverdue() =>
            _context.Tasks
                .Include(t => t.Board).ThenInclude(b => b.Project)
                .Include(t => t.AssignedEmployee)
                .Where(t => t.DueDate < DateTime.Today && t.Status != TaskStatus.Completed)
                .OrderBy(t => t.DueDate)
                .ToList();

        public ProjectTask? GetById(int id) =>
            _context.Tasks.FirstOrDefault(x => x.Id == id);

        public ProjectTask? GetDetails(int id) =>
            _context.Tasks
                .Include(t => t.Board).ThenInclude(b => b.Project)
                .Include(t => t.Sprint)
                .Include(t => t.AssignedEmployee)
                .Include(t => t.Comments)
                .Include(t => t.Attachments)
                .FirstOrDefault(x => x.Id == id);

        public void Add(ProjectTask task)
        {
            _context.Tasks.Add(task);
            _context.SaveChanges();
        }

        public void Update(ProjectTask task)
        {
            _context.Tasks.Update(task);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var task = GetById(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.Tasks.Any(x => x.Id == id);

        public int Count() => _context.Tasks.Count();

        public int CountByStatus(TaskStatus status) =>
            _context.Tasks.Count(t => t.Status == status);
    }
}
