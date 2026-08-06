namespace ProjectTracker.Models.Models.DTOs.Attachment
{
    public class AttachmentDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string UploadedBy { get; set; } = string.Empty;
        public int? TaskItemId { get; set; }
        public int? ProjectId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
