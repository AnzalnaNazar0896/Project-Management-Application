using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectTask = ProjectTracker.Models.Models.Entities.Tasks;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<ProjectTask> GetByBoardId(int boardId)
        {
            return _context.Tasks.Where(x => x.BoardId == boardId).OrderBy(x => x.DueDate).ToList();
        }

        public ProjectTask GetById(int id)
        {
            return _context.Tasks.FirstOrDefault(x => x.Id == id);
        }

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
            var task = _context.Tasks
                .FirstOrDefault(x => x.Id == id);

            if (task != null)
            {
                _context.Tasks.Remove(task);
                _context.SaveChanges();
            }
        }
    }
}
