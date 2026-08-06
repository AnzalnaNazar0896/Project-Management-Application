using ProjectTracker.Models.Models.DTOs.Attachment;
using ProjectTracker.Models.Models.DTOs.Board;
using ProjectTracker.Models.Models.DTOs.Comment;
using ProjectTracker.Models.Models.DTOs.Notification;
using ProjectTracker.Models.Models.DTOs.Project;
using ProjectTracker.Models.Models.DTOs.Sprint;
using ProjectTracker.Models.Models.DTOs.Task;
using ProjectTracker.Models.Models.Entities;
using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Core.Mapping
{
    public static class DtoMapper
    {
        public static ProjectSummaryDTO ToSummary(this Project project) => new()
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            Status = project.Status,
            Progress = project.Progress,
            IsCompleted = project.IsCompleted,
            StartDate = project.StartDate,
            EndDate = project.EndDate
        };

        public static TaskSummaryDTO ToSummary(this Tasks task)
        {
            var board = task.Board;
            return new TaskSummaryDTO
            {
                Id = task.Id,
                Title = task.Title,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                BoardId = task.BoardId,
                BoardName = board?.BoardName,
                SprintId = task.SprintId,
                SprintName = task.Sprint?.SprintName,
                ProjectId = board?.ProjectId ?? 0,
                ProjectName = board?.Project?.ProjectName,
                AssignedEmployeeId = task.AssignedEmployeeId,
                AssignedTo = task.AssignedEmployee?.FullName
            };
        }

        public static BoardSummaryDTO ToSummary(this Board board) => new()
        {
            Id = board.Id,
            BoardName = board.BoardName,
            ProjectId = board.ProjectId,
            ProjectName = board.Project?.ProjectName,
            TaskCount = board.Tasks?.Count ?? 0
        };

        public static SprintSummaryDTO ToSummary(this Sprint sprint, int completedTasks = 0, int totalTasks = 0)
        {
            var progress = totalTasks == 0 ? 0 : (int)Math.Round(completedTasks * 100.0 / totalTasks);
            return new SprintSummaryDTO
            {
                Id = sprint.Id,
                SprintName = sprint.SprintName,
                StartDate = sprint.StartDate,
                EndDate = sprint.EndDate,
                Status = sprint.Status.ToString(),
                ProjectId = sprint.ProjectId,
                ProjectName = sprint.Project?.ProjectName,
                Progress = progress,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks
            };
        }

        public static NotificationDTO ToDto(this Notification n) => new()
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            NotificationType = n.NotificationType,
            Receiver = n.Receiver,
            IsRead = n.IsRead,
            CreatedDate = n.CreatedDate
        };

        public static CommentDTO ToDto(this Comment c) => new()
        {
            Id = c.Id,
            Message = c.Message,
            CreatedBy = c.CreatedBy,
            TaskItemId = c.TaskItemId,
            CreatedDate = c.CreatedDate
        };

        public static AttachmentDTO ToDto(this Attachment a) => new()
        {
            Id = a.Id,
            FileName = a.FileName,
            FileType = a.FileType,
            FilePath = a.FilePath,
            UploadedBy = a.UploadedBy,
            TaskItemId = a.TaskItemId,
            ProjectId = a.ProjectId,
            CreatedDate = a.CreatedDate
        };

        public static bool IsPending(this TaskStatus status) =>
            status is TaskStatus.Pending or TaskStatus.Todo or TaskStatus.Blocked;

        public static bool IsInProgress(this TaskStatus status) =>
            status is TaskStatus.InProgress or TaskStatus.Review;

        public static bool IsCompleted(this TaskStatus status) =>
            status == TaskStatus.Completed;
    }
}
