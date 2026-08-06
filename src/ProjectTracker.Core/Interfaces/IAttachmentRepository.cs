using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Interfaces
{
    public interface IAttachmentRepository
    {
        List<Attachment> GetAll();
        List<Attachment> GetByTaskId(int taskId);
        List<Attachment> GetByProjectId(int projectId);
        Attachment? GetById(int id);
        void Add(Attachment attachment);
        void Update(Attachment attachment);
        void Delete(int id);
        bool Exists(int id);
        int Count();
    }
}
