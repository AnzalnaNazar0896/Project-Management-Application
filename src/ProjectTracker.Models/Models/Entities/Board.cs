namespace ProjectTracker.Models.Models.Entities
{
    public class Board: BaseEntity
    {
        public string BoardName { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
        public Board()
        {

        }
        public void RenameBoard(string newName)
        {
            BoardName = newName;
            UpdatedDate = DateTime.Now;
        }
    }
}
