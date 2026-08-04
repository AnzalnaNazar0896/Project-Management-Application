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
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;

        public NotificationRepository(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Notification> GetByReceiver(string receiver)
        {
            return _context.Notifications
                .Where(x => x.Receiver == receiver)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
        }

        public List<Notification> GetUnread(string receiver)
        {
            return _context.Notifications
                .Where(x =>
                    x.Receiver == receiver &&
                    !x.IsRead)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
        }

        public Notification GetById(int id)
        {
            return _context.Notifications
                .FirstOrDefault(x => x.Id == id);
        }

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
            var notification = _context.Notifications
                .FirstOrDefault(x => x.Id == id);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                _context.SaveChanges();
            }
        }
    }
}
