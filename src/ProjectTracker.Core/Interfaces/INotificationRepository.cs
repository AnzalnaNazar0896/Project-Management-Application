using ProjectTracker.Models.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Core.Interfaces
{
    public interface INotificationRepository
    {
        List<Notification> GetByReceiver(string receiver);

        List<Notification> GetUnread(string receiver);

        Notification GetById(int id);

        void Add(Notification notification);

        void Update(Notification notification);

        void Delete(int id);
    }
}
