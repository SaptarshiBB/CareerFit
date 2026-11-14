using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class QuestionOption
{
    public int Id { get; set; }

    public int QuestionId { get; set; }

    [ForeignKey(nameof(QuestionId))]
    public virtual Question? Question { get; set; }

    [Required]
    [StringLength(300)]
    public string OptionText { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string OptionValue { get; set; } = string.Empty;

    [StringLength(100)]
    public string? CategoryMapping { get; set; } // e.g. "Software Development", "Data Science", etc.

    [StringLength(100)]
    public string? SkillTag { get; set; } // e.g. "C#", "Python", "Problem Solving"

    public double ScoreWeight { get; set; } = 1.0;

    public int OrderNumber { get; set; } = 0;
}
