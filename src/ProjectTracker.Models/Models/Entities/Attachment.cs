namespace ProjectTracker.Models.Models.Entities
{
    public class Attachment : BaseEntity
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public int TaskItemId { get; set; }
        public Tasks TaskItem { get; set; }
        public Attachment()
        {

        }

        public void UpdateAttachment(string fileName, string filePath)
        {
            FileName = fileName;
            FilePath = filePath;
            UpdatedDate = DateTime.Now;
        }
    }
}
