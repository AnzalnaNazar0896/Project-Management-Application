using TaskStatus = ProjectTracker.Models.Models.Enums.TaskStatus;

namespace ProjectTracker.Models.Models.DTOs.Board
{
    public class KanbanMoveTaskDTO
    {
        public int TaskId { get; set; }
        public int BoardId { get; set; }
        public TaskStatus Status { get; set; }
    }
}
