using System.ComponentModel.DataAnnotations;

namespace CareerFit.Models;

public class Question
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string SectionKey { get; set; } = string.Empty; // "Education", "TechnicalSkills", "SoftSkills", "Interests", "ProblemSolving", "WorkEnvironment", "CareerGoals", "ExperienceLevel"

    [Required]
    [StringLength(100)]
    public string SectionTitle { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string QuestionType { get; set; } = "SingleChoice"; // "SingleChoice", "MultiSelect", "RatingScale"

    [StringLength(250)]
    public string? HelpText { get; set; }

    public int OrderNumber { get; set; } = 0;

    public bool IsRequired { get; set; } = true;

    // Navigation properties
    public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    public virtual ICollection<AssessmentAnswer> Answers { get; set; } = new List<AssessmentAnswer>();
}
