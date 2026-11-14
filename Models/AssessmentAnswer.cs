using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class AssessmentAnswer
{
    public int Id { get; set; }

    public int AssessmentId { get; set; }

    [ForeignKey(nameof(AssessmentId))]
    public virtual Assessment? Assessment { get; set; }

    public int QuestionId { get; set; }

    [ForeignKey(nameof(QuestionId))]
    public virtual Question? Question { get; set; }

    // Comma-separated or JSON string of chosen Option IDs
    public string SelectedOptionIds { get; set; } = string.Empty;

    // Human-readable string or comma-separated values (e.g. "Python, SQL, Docker")
    public string SelectedValues { get; set; } = string.Empty;

    // For rating scale questions (e.g. 1-5 or 1-10)
    public double? NumericScore { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
}
