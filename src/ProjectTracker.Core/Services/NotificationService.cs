using ProjectTracker.Core.Interfaces;
using ProjectTracker.Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Core.Services
{
    public class NotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(
            INotificationRepository repository)
        {
            _repository = repository;
        }

        public void Create(
            string message,
            string type,
            string receiver)
        {
            var notification = new Notification
            {
                Message = message,
                NotificationType = type,
                Receiver = receiver,
                IsRead = false,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _repository.Add(notification);
        }

        public List<Notification> GetUnread(string receiver)
        {
            return _repository.GetUnread(receiver);
        }

        public void MarkAsRead(int id)
        {
            var notification = _repository.GetById(id);

            if (notification != null)
            {
                notification.IsRead = true;

                _repository.Update(notification);
            }
        }
    }
}
