using System.ComponentModel.DataAnnotations;
using ProjectTracker.Models.Constants;

namespace ProjectTracker.Models.Models.DTOs.Team
{
    public class EditEmployeeUserDTO
    {
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [Display(Name = "Email (login)")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Department { get; set; }

        [Display(Name = "Availability")]
        public string Availability { get; set; } = "Available";

        [Required]
        [Display(Name = "Application role")]
        public string Role { get; set; } = AppRoles.Member;

        [Display(Name = "Projects")]
        public List<int> ProjectIds { get; set; } = new();

        public bool HasLogin { get; set; }
    }
}
