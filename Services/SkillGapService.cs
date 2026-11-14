using CareerFit.Data;
using CareerFit.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Services;

public class SkillGapService : ISkillGapService
{
    private readonly ApplicationDbContext _context;

    public SkillGapService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SkillGapAnalysisResult> AnalyzeSkillGapAsync(string userId, int careerId)
    {
        var career = await _context.Careers
            .Include(c => c.CareerSkills)
                .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == careerId)
            ?? throw new ArgumentException($"Career with Id {careerId} not found");

        var profile = await _context.CandidateProfiles
            .Include(p => p.UserSkills)
                .ThenInclude(us => us.Skill)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        // Also check if candidate selected skills in assessment
        var assessment = await _context.Assessments
            .Include(a => a.Answers)
                .ThenInclude(ans => ans.Question)
            .Where(a => a.UserId == userId && a.IsCompleted)
            .OrderByDescending(a => a.CompletedAt ?? a.CreatedAt)
            .FirstOrDefaultAsync();

        var userSkillsMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (profile != null)
        {
            foreach (var us in profile.UserSkills)
            {
                if (us.Skill != null)
                {
                    userSkillsMap[us.Skill.Name] = us.ProficiencyLevel;
                }
            }
        }

        if (assessment != null)
        {
            foreach (var ans in assessment.Answers.Where(a => a.Question?.SectionKey == "TechnicalSkills" || a.Question?.SectionKey == "SoftSkills"))
            {
                var values = ans.SelectedValues.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var val in values)
                {
                    if (!userSkillsMap.ContainsKey(val))
                    {
                        userSkillsMap[val] = "Intermediate";
                    }
                }
            }
        }

        var possessedList = new List<SkillGapItem>();
        var missingList = new List<SkillGapItem>();

        int totalSkills = career.CareerSkills.Count;
        double totalScoreWeight = 0;
        double earnedScoreWeight = 0;

        foreach (var cs in career.CareerSkills)
        {
            var skillName = cs.Skill?.Name ?? "Skill";
            var category = cs.Skill?.Category ?? "Technical";
            double weight = cs.Priority == "High" ? 3.0 : (cs.Priority == "Medium" ? 2.0 : 1.0);
            totalScoreWeight += weight;

            if (userSkillsMap.TryGetValue(skillName, out var candidateProf))
            {
                earnedScoreWeight += weight;
                possessedList.Add(new SkillGapItem
                {
                    SkillName = skillName,
                    Category = category,
                    IsPossessed = true,
                    IsRequired = cs.IsRequired,
                    Priority = cs.Priority,
                    RequiredProficiency = cs.MinProficiency,
                    CandidateProficiency = candidateProf,
                    RecommendationNotes = $"You meet or exceed the {cs.MinProficiency} benchmark."
                });
            }
            else
            {
                missingList.Add(new SkillGapItem
                {
                    SkillName = skillName,
                    Category = category,
                    IsPossessed = false,
                    IsRequired = cs.IsRequired,
                    Priority = cs.Priority,
                    RequiredProficiency = cs.MinProficiency,
                    CandidateProficiency = "Missing",
                    RecommendationNotes = cs.IsRequired
                        ? $"Essential requirement. Target {cs.MinProficiency} proficiency."
                        : $"Competitive advantage. Recommended for advanced projects."
                });
            }
        }

        double matchPct = totalScoreWeight > 0 ? (earnedScoreWeight / totalScoreWeight) * 100.0 : 0.0;

        var suggestedLearning = missingList
            .OrderByDescending(m => m.Priority == "High")
            .ThenByDescending(m => m.IsRequired)
            .Select(m => $"Target: {m.SkillName} ({m.Priority} Priority) - Aim for {m.RequiredProficiency} proficiency")
            .ToList();

        return new SkillGapAnalysisResult
        {
            Career = career,
            MatchPercentage = Math.Round(matchPct, 1),
            TotalRequiredSkills = totalSkills,
            PossessedSkillsCount = possessedList.Count,
            MissingSkillsCount = missingList.Count,
            PossessedSkills = possessedList,
            MissingSkills = missingList.OrderByDescending(m => m.Priority == "High").ThenByDescending(m => m.IsRequired).ToList(),
            SuggestedLearningPath = suggestedLearning
        };
    }
}
