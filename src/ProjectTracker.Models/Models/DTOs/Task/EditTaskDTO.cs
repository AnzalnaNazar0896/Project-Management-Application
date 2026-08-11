using System.ComponentModel.DataAnnotations;
using ProjectTracker.Models.Models.Entities;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Models.Models.DTOs.Task
{
    public class EditTaskDTO
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public TaskStatus Status { get; set; }
        public TaskPriority Priority { get; set; }
        public DateTime DueDate { get; set; }
        public int BoardId { get; set; }
        public int? SprintId { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public int ProjectId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
