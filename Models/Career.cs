using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class Career
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    public int? CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual CareerCategory? CareerCategory { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; } = string.Empty; // e.g. "Software Development", "Data Science"

    [Required]
    [StringLength(500)]
    public string Summary { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Responsibilities { get; set; } = string.Empty; // bullet points or newline-separated

    [Required]
    [StringLength(200)]
    public string RequiredEducation { get; set; } = "Bachelor's Degree in Computer Science or related field";

    [Required]
    [StringLength(150)]
    public string ExperienceLevel { get; set; } = "Entry-Level"; // "Entry-Level", "Mid-Level", "Senior"

    [Required]
    public string ExperienceRequirements { get; set; } = string.Empty;

    [Required]
    public string SuggestedLearningTopics { get; set; } = string.Empty;

    public decimal SalaryMin { get; set; }
    public decimal SalaryMax { get; set; }

    [StringLength(10)]
    public string SalaryCurrency { get; set; } = "USD";

    [StringLength(300)]
    public string SalaryNotes { get; set; } = "Illustrative compensation benchmark for full-time industry roles; actual compensation varies by location, industry, and credentials.";

    [Required]
    public string GrowthOpportunities { get; set; } = string.Empty;

    [StringLength(50)]
    public string DemandLevel { get; set; } = "High"; // "Very High", "High", "Moderate"

    [StringLength(50)]
    public string WorkEnvironment { get; set; } = "Hybrid"; // "Remote", "Hybrid", "On-site"

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ICollection<CareerSkill> CareerSkills { get; set; } = new List<CareerSkill>();
    public virtual ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
    public virtual ICollection<SavedCareer> SavedCareers { get; set; } = new List<SavedCareer>();
    public virtual ICollection<Roadmap> Roadmaps { get; set; } = new List<Roadmap>();
}
