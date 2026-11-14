using CareerFit.Models;
using CareerFit.Services;

namespace CareerFit.ViewModels;

public class UserDashboardViewModel
{
    public ApplicationUser User { get; set; } = null!;
    public CandidateProfile? Profile { get; set; }
    public int ProfileCompletionPercentage { get; set; }

    public Assessment? LatestAssessment { get; set; }
    public bool HasCompletedAssessment => LatestAssessment != null && LatestAssessment.IsCompleted;

    public CareerMatchResult? TopRecommendation { get; set; }
    public List<CareerMatchResult> Top5Recommendations { get; set; } = new();

    public List<SkillGapItem> KeyMissingSkills { get; set; } = new();

    public List<Roadmap> ActiveRoadmaps { get; set; } = new();
    public double AverageRoadmapProgress { get; set; }

    public List<SavedCareer> SavedCareers { get; set; } = new();
    public List<Assessment> AssessmentHistory { get; set; } = new();
}
