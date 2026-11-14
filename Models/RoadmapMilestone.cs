using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class RoadmapMilestone
{
    public int Id { get; set; }

    public int RoadmapId { get; set; }

    [ForeignKey(nameof(RoadmapId))]
    public virtual Roadmap? Roadmap { get; set; }

    [Required]
    [StringLength(50)]
    public string Stage { get; set; } = "Beginner"; // "Beginner", "Intermediate", "Advanced"

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    // Comma-separated list or formatted text of recommended skills
    [StringLength(300)]
    public string RecommendedSkills { get; set; } = string.Empty;

    // Suggested real-world project to build
    [StringLength(500)]
    public string SuggestedProjects { get; set; } = string.Empty;

    public int OrderNumber { get; set; } = 1;

    public bool IsCompleted { get; set; } = false;

    public DateTime? CompletedAt { get; set; }
}
