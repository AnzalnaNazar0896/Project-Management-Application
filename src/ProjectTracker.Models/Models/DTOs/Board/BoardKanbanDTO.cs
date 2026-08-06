using ProjectTracker.Models.Models.DTOs.Task;

namespace ProjectTracker.Models.Models.DTOs.Board
{
    public class BoardKanbanDTO
    {
        public int Id { get; set; }
        public string BoardName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public List<TaskSummaryDTO> Pending { get; set; } = new();
        public List<TaskSummaryDTO> InProgress { get; set; } = new();
        public List<TaskSummaryDTO> Completed { get; set; } = new();
    }
}
