using System.ComponentModel.DataAnnotations;
using CareerFit.Models;

namespace CareerFit.ViewModels;

public class CandidateProfileViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Full Name is required")]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email Address")]
    public string? Email { get; set; }

    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required]
    [Display(Name = "Highest Education")]
    public string HighestEducation { get; set; } = "Bachelor's Degree";

    [Display(Name = "Degree / Diploma")]
    public string Degree { get; set; } = string.Empty;

    [Display(Name = "Specialization / Major")]
    public string Specialization { get; set; } = string.Empty;

    [Display(Name = "Graduation Year")]
    [Range(1970, 2035, ErrorMessage = "Please enter a valid graduation year")]
    public int? GraduationYear { get; set; }

    [Display(Name = "Experience Level")]
    public string ExperienceLevel { get; set; } = "Entry-Level";

    [Display(Name = "Years of Professional Experience")]
    [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years")]
    public int YearsOfExperience { get; set; } = 0;

    [Display(Name = "Work Experience Summary")]
    [DataType(DataType.MultilineText)]
    public string? WorkExperience { get; set; }

    [Display(Name = "Career Interests (Select or type areas)")]
    public string CareerInterests { get; set; } = string.Empty;

    [Display(Name = "Preferred Work Domain")]
    public string PreferredWorkDomain { get; set; } = "Information Technology";

    [Display(Name = "Preferred Work Environment")]
    public string PreferredWorkEnvironment { get; set; } = "Hybrid";

    public int CompletionPercentage { get; set; }

    public List<UserSkill> CurrentSkills { get; set; } = new();

    // For adding new skill
    public AddSkillViewModel NewSkill { get; set; } = new();
}

public class AddSkillViewModel
{
    [Required(ErrorMessage = "Please select a skill")]
    public int SkillId { get; set; }

    [Required]
    public string ProficiencyLevel { get; set; } = "Intermediate";

    [Range(0, 30)]
    public int YearsOfExperience { get; set; } = 1;
}
