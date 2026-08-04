using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class SprintRepository : ISprintRepository
    {
        private readonly ApplicationDbContext _context;

        public SprintRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Sprint> GetByProjectId(int projectId)
        {
            return _context.Sprints
                .Where(x => x.ProjectId == projectId)
                .Include(x => x.Tasks)
                .OrderByDescending(x => x.StartDate)
                .ToList();
        }

        public Sprint GetById(int id)
        {
            return _context.Sprints
                .Include(x => x.Tasks)
                .FirstOrDefault(x => x.Id == id);
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
            var sprint = _context.Sprints
                .FirstOrDefault(x => x.Id == id);

            if (sprint != null)
            {
                _context.Sprints.Remove(sprint);
                _context.SaveChanges();
            }
        }
    }
}
