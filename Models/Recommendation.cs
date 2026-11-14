using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class Recommendation
{
    public int Id { get; set; }

    public int? AssessmentId { get; set; }

    [ForeignKey(nameof(AssessmentId))]
    public virtual Assessment? Assessment { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? User { get; set; }

    public int CareerId { get; set; }

    [ForeignKey(nameof(CareerId))]
    public virtual Career? Career { get; set; }

    // Weighted Overall Score (0.0 to 100.0)
    public double OverallScore { get; set; }

    // Normalized Component Scores (0.0 to 100.0)
    public double TechnicalSkillScore { get; set; } // 40% weight
    public double InterestScore { get; set; }       // 25% weight
    public double EducationScore { get; set; }      // 20% weight
    public double WorkPreferenceScore { get; set; } // 15% weight

    public int Rank { get; set; } = 1;

    // JSON or delimited lists of skills
    public string MatchingSkillsJson { get; set; } = "[]";
    public string MissingSkillsJson { get; set; } = "[]";

    // Explainable rationale
    public string Explanation { get; set; } = string.Empty;

    // Actionable improvement steps
    public string ImprovementSuggestions { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
