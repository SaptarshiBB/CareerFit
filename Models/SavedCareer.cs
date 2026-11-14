using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class SavedCareer
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public virtual ApplicationUser? User { get; set; }

    public int CareerId { get; set; }

    [ForeignKey(nameof(CareerId))]
    public virtual Career? Career { get; set; }

    public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    [StringLength(300)]
    public string? PersonalNotes { get; set; }
}
