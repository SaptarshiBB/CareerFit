using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class Roadmap
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? User { get; set; }

    public int CareerId { get; set; }

    [ForeignKey(nameof(CareerId))]
    public virtual Career? Career { get; set; }

    [Required]
    [StringLength(150)]
    public string TargetRole { get; set; } = string.Empty;

    [StringLength(500)]
    public string Summary { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdated { get; set; }

    // Navigation properties
    public virtual ICollection<RoadmapMilestone> Milestones { get; set; } = new List<RoadmapMilestone>();
}
