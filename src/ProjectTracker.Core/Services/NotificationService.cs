using Microsoft.Extensions.Logging;
using ProjectTracker.Core.Interfaces;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Services
{
    public class NotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly IEmailNotificationService _email;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository repository,
            IEmailNotificationService email,
            ILogger<NotificationService> logger)
        {
            _repository = repository;
            _email = email;
            _logger = logger;
        }

        public void Create(string title, string message, string type, string receiver)
        {
            _repository.Add(new Notification
            {
                Title = title,
                Message = message,
                NotificationType = type,
                Receiver = receiver,
                IsRead = false,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            });

            TrySendEmail(receiver, title, message);
        }

        public void Create(string message, string type, string receiver) =>
            Create(type, message, type, receiver);

        public void NotifyProjectCreated(string projectName, string receiver) =>
            Create("Project Created", $"You were added to project '{projectName}'.", "ProjectCreated", receiver);

        public void NotifyTaskAssigned(string taskTitle, string receiver) =>
            Create("Task Assigned", $"You have been assigned to task '{taskTitle}'.", "TaskAssigned", receiver);

        public void NotifyTaskCompleted(string taskTitle, string receiver) =>
            Create("Task Completed", $"Task '{taskTitle}' was marked completed.", "TaskCompleted", receiver);

        public void NotifyMemberAdded(string projectName, string receiver, string actor) =>
            Create("Member Added", $"{actor} added you to project '{projectName}'.", "MemberAdded", receiver);

        public void NotifySprintCreated(string sprintName, string receiver) =>
            Create("Sprint Created", $"Sprint '{sprintName}' has been created.", "SprintCreated", receiver);

        public List<Notification> GetAll() => _repository.GetAll();

        public List<Notification> GetByReceiver(string receiver) => _repository.GetByReceiver(receiver);

        public List<Notification> GetRecent(int take) => _repository.GetRecent(take);

        public List<Notification> GetUnread(string receiver) => _repository.GetUnread(receiver);

        public void MarkAsRead(int id)
        {
            var notification = _repository.GetById(id);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.UpdatedDate = DateTime.Now;
                _repository.Update(notification);
            }
        }

        public int Count() => _repository.Count();

        private void TrySendEmail(string receiver, string subject, string body)
        {
            if (!_email.IsEnabled || string.IsNullOrWhiteSpace(receiver) || !receiver.Contains('@'))
                return;

            try
            {
                _email.SendAsync(receiver, subject, $"<p>{body}</p>").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send email notification to {Receiver}", receiver);
            }
        }
    }
}
