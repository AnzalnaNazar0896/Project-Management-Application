namespace ProjectTracker.Models.Models.DTOs.Comment
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public int TaskItemId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
