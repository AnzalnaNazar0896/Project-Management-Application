namespace ProjectTracker.Core.Interfaces
{
    public interface IEmailNotificationService
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
        bool IsEnabled { get; }
    }
}
