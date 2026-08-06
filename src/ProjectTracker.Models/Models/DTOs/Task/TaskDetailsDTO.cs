using ProjectTracker.Models.Models.DTOs.Attachment;
using ProjectTracker.Models.Models.DTOs.Comment;

namespace ProjectTracker.Models.Models.DTOs.Task
{
    public class TaskDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public int BoardId { get; set; }
        public string BoardName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int? SprintId { get; set; }
        public string? SprintName { get; set; }
        public int? AssignedEmployeeId { get; set; }
        public string? AssignedTo { get; set; }
        public List<CommentDTO> Comments { get; set; } = new();
        public List<AttachmentDTO> Attachments { get; set; } = new();
    }
}
