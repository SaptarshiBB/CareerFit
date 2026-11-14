using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class Assessment
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? User { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public bool IsCompleted { get; set; } = false;

    [StringLength(250)]
    public string? SummaryTitle { get; set; }

    // Navigation properties
    public virtual ICollection<AssessmentAnswer> Answers { get; set; } = new List<AssessmentAnswer>();
    public virtual ICollection<Recommendation> Recommendations { get; set; } = new List<Recommendation>();
}
