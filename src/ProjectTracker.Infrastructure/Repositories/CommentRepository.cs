using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Comment> GetAll() =>
            _context.Comments
                .Include(c => c.Employee)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

        public List<Comment> GetByTaskId(int taskId) =>
            _context.Comments
                .Where(c => c.TaskItemId == taskId)
                .Include(c => c.Employee)
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

        public Comment? GetById(int id) =>
            _context.Comments.FirstOrDefault(x => x.Id == id);

        public void Add(Comment comment)
        {
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public void Update(Comment comment)
        {
            _context.Comments.Update(comment);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var comment = GetById(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.Comments.Any(x => x.Id == id);

        public int Count() => _context.Comments.Count();
    }
}
