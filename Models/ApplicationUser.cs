using Microsoft.AspNetCore.Identity;

namespace CareerFit.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual CandidateProfile? CandidateProfile { get; set; }
    public virtual ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
    public virtual ICollection<SavedCareer> SavedCareers { get; set; } = new List<SavedCareer>();
    public virtual ICollection<Roadmap> Roadmaps { get; set; } = new List<Roadmap>();
}
