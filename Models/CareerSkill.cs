using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class CareerSkill
{
    public int CareerId { get; set; }

    [ForeignKey(nameof(CareerId))]
    public virtual Career? Career { get; set; }

    public int SkillId { get; set; }

    [ForeignKey(nameof(SkillId))]
    public virtual Skill? Skill { get; set; }

    public bool IsRequired { get; set; } = true;

    [Required]
    [StringLength(20)]
    public string Priority { get; set; } = "High"; // "High", "Medium", "Low"

    [StringLength(30)]
    public string MinProficiency { get; set; } = "Intermediate"; // "Beginner", "Intermediate", "Advanced"
}
