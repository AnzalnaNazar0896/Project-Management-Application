namespace ProjectTracker.Models.Models.Entities
{
    public class Comment : BaseEntity
    {
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public int TaskItemId { get; set; }
        public Tasks TaskItem { get; set; }

        public Comment()
        {

        }

        public void UpdateComment(string newMessage)
        {
            Message = newMessage;
            UpdatedDate = DateTime.Now;
        }
    }
}
