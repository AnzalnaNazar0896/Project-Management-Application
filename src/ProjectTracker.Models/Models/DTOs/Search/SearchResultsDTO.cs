namespace ProjectTracker.Models.Models.DTOs.Search
{
    public class SearchResultsDTO
    {
        public string Query { get; set; } = string.Empty;
        public List<SearchItemDTO> Projects { get; set; } = new();
        public List<SearchItemDTO> Tasks { get; set; } = new();
        public List<SearchItemDTO> Boards { get; set; } = new();
        public List<SearchItemDTO> Sprints { get; set; } = new();
        public List<SearchItemDTO> Employees { get; set; } = new();
    }

    public class SearchItemDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Subtitle { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}
