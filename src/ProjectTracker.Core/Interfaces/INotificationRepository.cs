using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Interfaces
{
    public interface INotificationRepository
    {
        List<Notification> GetAll();
        List<Notification> GetByReceiver(string receiver);
        List<Notification> GetRecent(int take);
        List<Notification> GetUnread(string receiver);
        Notification? GetById(int id);
        void Add(Notification notification);
        void Update(Notification notification);
        void Delete(int id);
        bool Exists(int id);
        int Count();
    }
}
