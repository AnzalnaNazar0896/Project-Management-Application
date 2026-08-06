namespace ProjectTracker.Models.Models.Entities
{
    public class Attachment : BaseEntity
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public string UploadedBy { get; set; } = string.Empty;
        public int? TaskItemId { get; set; }
        public Tasks? TaskItem { get; set; }
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }

        public void UpdateAttachment(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
            UpdatedDate = DateTime.Now;
        }
    }
}
