using ProjectTracker.Core.Interfaces;
using ProjectTracker.Interfaces;
using ProjectTracker.Models;
using ProjectTracker.Models.Models.DTOs.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectTracker.Core.Services
{
    public class TaskService
    {
        private readonly ITaskRepository _repository;

        public TaskService(ITaskRepository repository)
        {
            _repository = repository;
        }

        public List<Task> GetBoardTasks(int boardId)
        {
            return _repository.GetByBoardId(boardId);
        }

        public void CreateTask(CreateTaskDTO model)
        {
            var task = new Tasks
            {
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Priority = model.Priority,
                DueDate = model.DueDate,
                BoardId = model.BoardId,
                SprintId = model.SprintId,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now
            };

            _repository.Add(task);
        }
    }
}
