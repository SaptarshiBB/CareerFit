using CareerFit.Models;

namespace CareerFit.Services;

public class SkillGapItem
{
    public string SkillName { get; set; } = string.Empty;
    public string Category { get; set; } = "Technical";
    public bool IsPossessed { get; set; }
    public bool IsRequired { get; set; }
    public string Priority { get; set; } = "High"; // High, Medium, Low
    public string RequiredProficiency { get; set; } = "Intermediate";
    public string CandidateProficiency { get; set; } = "None";
    public string RecommendationNotes { get; set; } = string.Empty;
}

public class SkillGapAnalysisResult
{
    public Career Career { get; set; } = null!;
    public double MatchPercentage { get; set; }
    public int TotalRequiredSkills { get; set; }
    public int PossessedSkillsCount { get; set; }
    public int MissingSkillsCount { get; set; }

    public List<SkillGapItem> PossessedSkills { get; set; } = new();
    public List<SkillGapItem> MissingSkills { get; set; } = new();
    public List<string> SuggestedLearningPath { get; set; } = new();
}

public interface ISkillGapService
{
    Task<SkillGapAnalysisResult> AnalyzeSkillGapAsync(string userId, int careerId);
}
