using ProjectTracker.Models.Models.Entities;
using System.ComponentModel.DataAnnotations;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Models.Models.DTOs.Task
{
    public class CreateTaskDTO
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);
        public int BoardId { get; set; }
        public int? SprintId { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public int? ProjectId { get; set; }
    }
}
