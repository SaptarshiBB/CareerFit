using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class CandidateProfile
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? User { get; set; }

    [Required]
    [StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(150)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    public string HighestEducation { get; set; } = "Bachelor's Degree"; // High School, Associate, Bachelor's, Master's, Doctorate

    [StringLength(120)]
    public string Degree { get; set; } = string.Empty;

    [StringLength(120)]
    public string Specialization { get; set; } = string.Empty;

    public int? GraduationYear { get; set; }

    [StringLength(1000)]
    public string? WorkExperience { get; set; }

    [StringLength(50)]
    public string ExperienceLevel { get; set; } = "Entry-Level"; // Entry-Level (0-2 yrs), Mid-Level (3-5 yrs), Senior (5+ yrs)

    public int YearsOfExperience { get; set; } = 0;

    [StringLength(500)]
    public string CareerInterests { get; set; } = string.Empty; // e.g., "Software Development, Cloud Computing"

    [StringLength(100)]
    public string PreferredWorkDomain { get; set; } = "Information Technology";

    [StringLength(50)]
    public string PreferredWorkEnvironment { get; set; } = "Hybrid"; // Remote, Hybrid, On-site, Flexible

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}
