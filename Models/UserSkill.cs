using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerFit.Models;

public class UserSkill
{
    public int Id { get; set; }

    public int CandidateProfileId { get; set; }

    [ForeignKey(nameof(CandidateProfileId))]
    public virtual CandidateProfile? CandidateProfile { get; set; }

    public int SkillId { get; set; }

    [ForeignKey(nameof(SkillId))]
    public virtual Skill? Skill { get; set; }

    [Required]
    [StringLength(30)]
    public string ProficiencyLevel { get; set; } = "Intermediate"; // "Beginner", "Intermediate", "Advanced"

    public int YearsOfExperience { get; set; } = 1;
}
