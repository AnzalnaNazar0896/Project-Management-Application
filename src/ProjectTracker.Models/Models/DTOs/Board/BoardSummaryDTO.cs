namespace ProjectTracker.Models.Models.DTOs.Board
{
    public class BoardSummaryDTO
    {
        public int Id { get; set; }
        public string BoardName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int TaskCount { get; set; }
    }
}
