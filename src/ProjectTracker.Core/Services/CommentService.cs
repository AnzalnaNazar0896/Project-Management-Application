using ProjectTracker.Core.Interfaces;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Core.Services
{
    public class CommentService
    {
        private readonly ICommentRepository _repository;

        public CommentService(ICommentRepository repository)
        {
            _repository = repository;
        }

        public List<Comment> GetAll() => _repository.GetAll();

        public List<Comment> GetByTaskId(int taskId) => _repository.GetByTaskId(taskId);

        public void Add(int taskId, string message, string createdBy, int? employeeId)
        {
            _repository.Add(new Comment
            {
                TaskItemId = taskId,
                Message = message,
                CreatedBy = createdBy,
                EmployeeId = employeeId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            });
        }

        public int Count() => _repository.Count();
    }
}
