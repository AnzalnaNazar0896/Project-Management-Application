using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Interfaces
{
    public interface ICommentRepository
    {
        List<Comment> GetAll();
        List<Comment> GetByTaskId(int taskId);
        Comment? GetById(int id);
        void Add(Comment comment);
        void Update(Comment comment);
        void Delete(int id);
        bool Exists(int id);
        int Count();
    }
}
