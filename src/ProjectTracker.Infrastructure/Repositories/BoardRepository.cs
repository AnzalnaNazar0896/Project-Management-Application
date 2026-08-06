using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class BoardRepository : IBoardRepository
    {
        private readonly ApplicationDbContext _context;

        public BoardRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Board> GetAll() =>
            _context.Boards
                .Include(x => x.Project)
                .Include(x => x.Tasks)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

        public List<Board> GetByProjectId(int projectId) =>
            _context.Boards
                .Where(x => x.ProjectId == projectId)
                .Include(x => x.Tasks)
                .ToList();

        public Board? GetById(int id) =>
            _context.Boards
                .Include(x => x.Project)
                .Include(x => x.Tasks)
                .ThenInclude(t => t.AssignedEmployee)
                .FirstOrDefault(x => x.Id == id);

        public void Add(Board board)
        {
            _context.Boards.Add(board);
            _context.SaveChanges();
        }

        public void Update(Board board)
        {
            _context.Boards.Update(board);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var board = _context.Boards.FirstOrDefault(x => x.Id == id);
            if (board != null)
            {
                _context.Boards.Remove(board);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.Boards.Any(x => x.Id == id);

        public int Count() => _context.Boards.Count();
    }
}
