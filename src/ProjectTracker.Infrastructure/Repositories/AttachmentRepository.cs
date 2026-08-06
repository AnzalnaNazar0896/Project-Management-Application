using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Attachment> GetAll() =>
            _context.Attachments.OrderByDescending(a => a.CreatedDate).ToList();

        public List<Attachment> GetByTaskId(int taskId) =>
            _context.Attachments.Where(a => a.TaskItemId == taskId).ToList();

        public List<Attachment> GetByProjectId(int projectId) =>
            _context.Attachments.Where(a => a.ProjectId == projectId).ToList();

        public Attachment? GetById(int id) =>
            _context.Attachments.FirstOrDefault(x => x.Id == id);

        public void Add(Attachment attachment)
        {
            _context.Attachments.Add(attachment);
            _context.SaveChanges();
        }

        public void Update(Attachment attachment)
        {
            _context.Attachments.Update(attachment);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var attachment = GetById(id);
            if (attachment != null)
            {
                _context.Attachments.Remove(attachment);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.Attachments.Any(x => x.Id == id);

        public int Count() => _context.Attachments.Count();
    }
}
