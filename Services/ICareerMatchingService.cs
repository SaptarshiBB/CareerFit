using CareerFit.Models;

namespace CareerFit.Services;

public class SkillMatchDetail
{
    public string SkillName { get; set; } = string.Empty;
    public string Category { get; set; } = "Technical";
    public bool IsPossessed { get; set; }
    public bool IsRequired { get; set; }
    public string Priority { get; set; } = "High";
    public string RequiredProficiency { get; set; } = "Intermediate";
    public string CandidateProficiency { get; set; } = "None";
}

public class CareerMatchResult
{
    public Career Career { get; set; } = null!;
    public double OverallScore { get; set; }
    public double TechnicalSkillScore { get; set; } // 40%
    public double InterestScore { get; set; }       // 25%
    public double EducationScore { get; set; }      // 20%
    public double WorkPreferenceScore { get; set; } // 15%
    public int Rank { get; set; }

    public List<SkillMatchDetail> MatchingSkills { get; set; } = new();
    public List<SkillMatchDetail> MissingSkills { get; set; } = new();

    public string Explanation { get; set; } = string.Empty;
    public string ImprovementSuggestions { get; set; } = string.Empty;
}

public interface ICareerMatchingService
{
    Task<List<CareerMatchResult>> CalculateRecommendationsAsync(string userId, int? assessmentId = null);
    Task<CareerMatchResult?> CalculateSingleCareerMatchAsync(string userId, int careerId);
    Task<bool> HasSufficientDataAsync(string userId);
}
