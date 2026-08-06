using System.ComponentModel.DataAnnotations;
using ProjectTracker.Models.Models.Entities;

namespace ProjectTracker.Models.Models.DTOs.Sprint
{
    public class EditSprintDTO
    {
        public int Id { get; set; }

        [Required]
        public string SprintName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public SprintStatus Status { get; set; }
        public int ProjectId { get; set; }
    }
}
