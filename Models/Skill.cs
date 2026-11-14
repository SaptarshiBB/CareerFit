using System.ComponentModel.DataAnnotations;

namespace CareerFit.Models;

public class Skill
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Category { get; set; } = "Technical"; // "Technical", "Soft"

    [StringLength(300)]
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<CareerSkill> CareerSkills { get; set; } = new List<CareerSkill>();
    public virtual ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
}
