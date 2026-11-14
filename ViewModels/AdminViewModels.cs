using System.ComponentModel.DataAnnotations;
using CareerFit.Models;
using CareerFit.Services;

namespace CareerFit.ViewModels;

public class AdminDashboardViewModel
{
    public AdminStatsDto Stats { get; set; } = new();
}

public class CareerEditViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = "Software Development";

    [Required]
    [StringLength(500)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Responsibilities { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string RequiredEducation { get; set; } = "Bachelor's Degree";

    [Required]
    public string ExperienceLevel { get; set; } = "Entry-Level";

    [Required]
    public string ExperienceRequirements { get; set; } = string.Empty;

    [Required]
    public string SuggestedLearningTopics { get; set; } = string.Empty;

    [Required]
    [Range(20000, 500000)]
    public decimal SalaryMin { get; set; } = 70000;

    [Required]
    [Range(20000, 500000)]
    public decimal SalaryMax { get; set; } = 120000;

    [Required]
    public string GrowthOpportunities { get; set; } = string.Empty;

    [Required]
    public string DemandLevel { get; set; } = "High";

    [Required]
    public string WorkEnvironment { get; set; } = "Hybrid";

    public bool IsActive { get; set; } = true;

    public List<int> SelectedSkillIds { get; set; } = new();
    public List<Skill> AvailableSkills { get; set; } = new();
    public List<string> AvailableCategories { get; set; } = new();
}

public class SkillEditViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = "Technical";

    [StringLength(300)]
    public string? Description { get; set; }
}
