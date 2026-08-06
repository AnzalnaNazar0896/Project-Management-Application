using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.Models.Models.DTOs.Project
{
    public class CreateProjectDTO
    {
        [Required]
        public string ProjectName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = "Active";

        public List<int> MemberIds { get; set; } = new();
    }
}
