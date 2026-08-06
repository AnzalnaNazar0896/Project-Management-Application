using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Data;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Notification> GetAll() =>
            _context.Notifications.OrderByDescending(x => x.CreatedDate).ToList();

        public List<Notification> GetByReceiver(string receiver) =>
            _context.Notifications
                .Where(x => x.Receiver == receiver)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

        public List<Notification> GetRecent(int take) =>
            _context.Notifications
                .OrderByDescending(x => x.CreatedDate)
                .Take(take)
                .ToList();

        public List<Notification> GetUnread(string receiver) =>
            _context.Notifications
                .Where(x => x.Receiver == receiver && !x.IsRead)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();

        public Notification? GetById(int id) =>
            _context.Notifications.FirstOrDefault(x => x.Id == id);

        public void Add(Notification notification)
        {
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }

        public void Update(Notification notification)
        {
            _context.Notifications.Update(notification);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var notification = GetById(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                _context.SaveChanges();
            }
        }

        public bool Exists(int id) => _context.Notifications.Any(x => x.Id == id);

        public int Count() => _context.Notifications.Count();
    }
}
