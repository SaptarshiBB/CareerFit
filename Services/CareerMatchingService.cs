using System.Text.Json;
using CareerFit.Data;
using CareerFit.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Services;

public class CareerMatchingService : ICareerMatchingService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CareerMatchingService> _logger;

    public CareerMatchingService(ApplicationDbContext context, ILogger<CareerMatchingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> HasSufficientDataAsync(string userId)
    {
        var profile = await _context.CandidateProfiles
            .Include(p => p.UserSkills)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        var hasCompletedAssessment = await _context.Assessments
            .AnyAsync(a => a.UserId == userId && a.IsCompleted);

        if (hasCompletedAssessment)
            return true;

        if (profile != null && profile.UserSkills.Any())
            return true;

        return false;
    }

    public async Task<List<CareerMatchResult>> CalculateRecommendationsAsync(string userId, int? assessmentId = null)
    {
        // 1. Fetch Candidate Profile and User Skills
        var profile = await _context.CandidateProfiles
            .Include(p => p.UserSkills)
                .ThenInclude(us => us.Skill)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        // 2. Fetch Latest or Specified Assessment Answers
        Assessment? assessment = null;
        if (assessmentId.HasValue)
        {
            assessment = await _context.Assessments
                .Include(a => a.Answers)
                    .ThenInclude(ans => ans.Question)
                .FirstOrDefaultAsync(a => a.Id == assessmentId.Value && a.UserId == userId);
        }
        else
        {
            assessment = await _context.Assessments
                .Include(a => a.Answers)
                    .ThenInclude(ans => ans.Question)
                .Where(a => a.UserId == userId && a.IsCompleted)
                .OrderByDescending(a => a.CompletedAt ?? a.CreatedAt)
                .FirstOrDefaultAsync();
        }

        // 3. Aggregate Candidate Information
        var candidateSkills = new Dictionary<string, (string Category, string Proficiency)>(StringComparer.OrdinalIgnoreCase);

        // Add from profile
        if (profile != null)
        {
            foreach (var us in profile.UserSkills)
            {
                if (us.Skill != null)
                {
                    candidateSkills[us.Skill.Name] = (us.Skill.Category, us.ProficiencyLevel);
                }
            }
        }

        // Add from assessment answers
        var candidateInterests = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string candidateEducation = profile?.HighestEducation ?? "Bachelor's Degree";
        string candidateDegree = profile?.Degree ?? "";
        string candidateWorkEnvironment = profile?.PreferredWorkEnvironment ?? "Hybrid";
        string candidateExperienceLevel = profile?.ExperienceLevel ?? "Entry-Level";

        if (profile != null && !string.IsNullOrWhiteSpace(profile.CareerInterests))
        {
            foreach (var item in profile.CareerInterests.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                candidateInterests.Add(item);
            }
        }

        if (assessment != null)
        {
            foreach (var ans in assessment.Answers)
            {
                var secKey = ans.Question?.SectionKey;
                if (string.IsNullOrEmpty(secKey)) continue;

                if (secKey == "TechnicalSkills")
                {
                    var values = ans.SelectedValues.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var val in values)
                    {
                        if (!candidateSkills.ContainsKey(val))
                        {
                            candidateSkills[val] = ("Technical", "Intermediate");
                        }
                    }
                }
                else if (secKey == "SoftSkills")
                {
                    var values = ans.SelectedValues.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var val in values)
                    {
                        if (!candidateSkills.ContainsKey(val))
                        {
                            candidateSkills[val] = ("Soft", "Intermediate");
                        }
                    }
                }
                else if (secKey == "Interests")
                {
                    if (!string.IsNullOrWhiteSpace(ans.SelectedValues))
                    {
                        candidateInterests.Add(ans.SelectedValues.Trim());
                    }
                }
                else if (secKey == "Education")
                {
                    if (!string.IsNullOrWhiteSpace(ans.SelectedValues) && ans.SelectedValues != "CS_IT")
                    {
                        candidateEducation = ans.SelectedValues.Trim();
                    }
                }
                else if (secKey == "WorkEnvironment")
                {
                    if (!string.IsNullOrWhiteSpace(ans.SelectedValues))
                    {
                        candidateWorkEnvironment = ans.SelectedValues.Trim();
                    }
                }
                else if (secKey == "ExperienceLevel")
                {
                    if (!string.IsNullOrWhiteSpace(ans.SelectedValues))
                    {
                        candidateExperienceLevel = ans.SelectedValues.Trim();
                    }
                }
            }
        }

        // 4. Fetch All Active Careers with required skills
        var careers = await _context.Careers
            .Where(c => c.IsActive)
            .Include(c => c.CareerSkills)
                .ThenInclude(cs => cs.Skill)
            .ToListAsync();

        var matchResults = new List<CareerMatchResult>();

        foreach (var career in careers)
        {
            var result = CalculateMatch(career, candidateSkills, candidateInterests, candidateEducation, candidateDegree, candidateWorkEnvironment, candidateExperienceLevel);
            matchResults.Add(result);
        }

        // Sort descending by OverallScore
        var sortedResults = matchResults.OrderByDescending(r => r.OverallScore).ToList();

        // Assign ranks
        for (int i = 0; i < sortedResults.Count; i++)
        {
            sortedResults[i].Rank = i + 1;
        }

        // Persist top recommendations to database for traceability
        try
        {
            // Remove previous recommendations for this assessment/user if re-calculating
            if (assessment != null)
            {
                var existingRecs = await _context.Recommendations
                    .Where(r => r.AssessmentId == assessment.Id)
                    .ToListAsync();
                _context.Recommendations.RemoveRange(existingRecs);

                var top5 = sortedResults.Take(5).ToList();
                foreach (var top in top5)
                {
                    var rec = new Recommendation
                    {
                        AssessmentId = assessment.Id,
                        UserId = userId,
                        CareerId = top.Career.Id,
                        OverallScore = Math.Round(top.OverallScore, 1),
                        TechnicalSkillScore = Math.Round(top.TechnicalSkillScore, 1),
                        InterestScore = Math.Round(top.InterestScore, 1),
                        EducationScore = Math.Round(top.EducationScore, 1),
                        WorkPreferenceScore = Math.Round(top.WorkPreferenceScore, 1),
                        Rank = top.Rank,
                        MatchingSkillsJson = JsonSerializer.Serialize(top.MatchingSkills.Select(m => m.SkillName).ToList()),
                        MissingSkillsJson = JsonSerializer.Serialize(top.MissingSkills.Select(m => new { m.SkillName, m.Priority, m.Category }).ToList()),
                        Explanation = top.Explanation,
                        ImprovementSuggestions = top.ImprovementSuggestions,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Recommendations.Add(rec);
                }
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error persisting career recommendations for user {UserId}", userId);
        }

        return sortedResults;
    }

    public async Task<CareerMatchResult?> CalculateSingleCareerMatchAsync(string userId, int careerId)
    {
        var career = await _context.Careers
            .Include(c => c.CareerSkills)
                .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == careerId);

        if (career == null) return null;

        var recs = await CalculateRecommendationsAsync(userId);
        return recs.FirstOrDefault(r => r.Career.Id == careerId);
    }

    private CareerMatchResult CalculateMatch(
        Career career,
        Dictionary<string, (string Category, string Proficiency)> candidateSkills,
        HashSet<string> candidateInterests,
        string candidateEducation,
        string candidateDegree,
        string candidateWorkEnvironment,
        string candidateExperienceLevel)
    {
        var matchingSkills = new List<SkillMatchDetail>();
        var missingSkills = new List<SkillMatchDetail>();

        // 1. Technical Skills Match (40% Weight)
        double technicalSkillScore = 0.0;
        var careerSkills = career.CareerSkills.ToList();

        if (careerSkills.Any())
        {
            double totalWeight = 0.0;
            double earnedWeight = 0.0;

            foreach (var cs in careerSkills)
            {
                var skillName = cs.Skill?.Name ?? "";
                var skillCategory = cs.Skill?.Category ?? "Technical";
                double weight = cs.Priority switch
                {
                    "High" => cs.IsRequired ? 3.0 : 2.0,
                    "Medium" => cs.IsRequired ? 2.0 : 1.5,
                    _ => 1.0
                };
                totalWeight += weight;

                if (candidateSkills.TryGetValue(skillName, out var candSkill))
                {
                    earnedWeight += weight;
                    matchingSkills.Add(new SkillMatchDetail
                    {
                        SkillName = skillName,
                        Category = skillCategory,
                        IsPossessed = true,
                        IsRequired = cs.IsRequired,
                        Priority = cs.Priority,
                        RequiredProficiency = cs.MinProficiency,
                        CandidateProficiency = candSkill.Proficiency
                    });
                }
                else
                {
                    missingSkills.Add(new SkillMatchDetail
                    {
                        SkillName = skillName,
                        Category = skillCategory,
                        IsPossessed = false,
                        IsRequired = cs.IsRequired,
                        Priority = cs.Priority,
                        RequiredProficiency = cs.MinProficiency,
                        CandidateProficiency = "None"
                    });
                }
            }

            technicalSkillScore = totalWeight > 0 ? (earnedWeight / totalWeight) * 100.0 : 50.0;
        }
        else
        {
            technicalSkillScore = 60.0; // Default baseline if career has no explicit skills
        }

        // 2. Interest Match (25% Weight)
        double interestScore = 40.0; // Baseline
        if (candidateInterests.Any())
        {
            if (candidateInterests.Contains(career.Category))
            {
                interestScore = 100.0;
            }
            else if (candidateInterests.Any(ci => career.Title.Contains(ci, StringComparison.OrdinalIgnoreCase) ||
                                                 ci.Contains(career.Category, StringComparison.OrdinalIgnoreCase)))
            {
                interestScore = 90.0;
            }
            else if (IsRelatedCategory(career.Category, candidateInterests))
            {
                interestScore = 75.0;
            }
            else
            {
                interestScore = 50.0;
            }
        }

        // 3. Education Match (20% Weight)
        double educationScore = 65.0; // Default baseline
        var eduLower = candidateEducation.ToLowerInvariant();
        if (eduLower.Contains("master") || eduLower.Contains("doctorate") || eduLower.Contains("phd"))
        {
            educationScore = 100.0;
        }
        else if (eduLower.Contains("bachelor"))
        {
            educationScore = career.RequiredEducation.Contains("Master", StringComparison.OrdinalIgnoreCase) ? 80.0 : 95.0;
        }
        else if (eduLower.Contains("associate"))
        {
            educationScore = 75.0;
        }
        else
        {
            educationScore = 60.0;
        }

        // Minor bonus for relevant degree/specialization keyword match
        if (!string.IsNullOrWhiteSpace(candidateDegree) &&
            (career.RequiredEducation.Contains(candidateDegree, StringComparison.OrdinalIgnoreCase) ||
             career.Category.Contains(candidateDegree, StringComparison.OrdinalIgnoreCase)))
        {
            educationScore = Math.Min(100.0, educationScore + 10.0);
        }

        // 4. Work Preference Match (15% Weight)
        double workPreferenceScore = 70.0;
        if (string.Equals(candidateWorkEnvironment, career.WorkEnvironment, StringComparison.OrdinalIgnoreCase))
        {
            workPreferenceScore = 100.0;
        }
        else if (string.Equals(candidateWorkEnvironment, "Hybrid", StringComparison.OrdinalIgnoreCase) ||
                 string.Equals(career.WorkEnvironment, "Hybrid", StringComparison.OrdinalIgnoreCase))
        {
            workPreferenceScore = 85.0;
        }
        else if (string.Equals(candidateWorkEnvironment, "Flexible", StringComparison.OrdinalIgnoreCase))
        {
            workPreferenceScore = 90.0;
        }
        else
        {
            workPreferenceScore = 60.0;
        }

        // Experience alignment adjustment (+- 5% normalized within bounds)
        if (string.Equals(candidateExperienceLevel, career.ExperienceLevel, StringComparison.OrdinalIgnoreCase))
        {
            workPreferenceScore = Math.Min(100.0, workPreferenceScore + 5.0);
        }

        // Weighted Overall Score Formula:
        // 40% Technical Skill + 25% Interest + 20% Education + 15% Work Preference
        double overallScore = (technicalSkillScore * 0.40) +
                              (interestScore * 0.25) +
                              (educationScore * 0.20) +
                              (workPreferenceScore * 0.15);

        overallScore = Math.Clamp(overallScore, 0.0, 100.0);

        // Generate explainable rationale
        string explanation = GenerateExplanation(career, overallScore, technicalSkillScore, interestScore, matchingSkills, missingSkills);

        // Generate actionable improvement suggestions
        string suggestions = GenerateImprovementSuggestions(career, missingSkills);

        return new CareerMatchResult
        {
            Career = career,
            OverallScore = overallScore,
            TechnicalSkillScore = technicalSkillScore,
            InterestScore = interestScore,
            EducationScore = educationScore,
            WorkPreferenceScore = workPreferenceScore,
            MatchingSkills = matchingSkills,
            MissingSkills = missingSkills.OrderByDescending(m => m.Priority == "High").ThenByDescending(m => m.IsRequired).ToList(),
            Explanation = explanation,
            ImprovementSuggestions = suggestions
        };
    }

    private static bool IsRelatedCategory(string careerCategory, HashSet<string> candidateInterests)
    {
        var relatedMap = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["Software Development"] = new[] { "Cloud Computing", "Cybersecurity", "Data Science" },
            ["Cloud Computing"] = new[] { "Software Development", "Cybersecurity" },
            ["Cybersecurity"] = new[] { "Cloud Computing", "Software Development" },
            ["Data Science"] = new[] { "Software Development", "Finance" },
            ["Business and Management"] = new[] { "Finance", "Marketing" },
            ["Finance"] = new[] { "Business and Management", "Data Science" },
            ["Design"] = new[] { "Marketing", "Software Development" },
            ["Marketing"] = new[] { "Design", "Business and Management" }
        };

        if (relatedMap.TryGetValue(careerCategory, out var relatedList))
        {
            return relatedList.Any(candidateInterests.Contains);
        }

        return false;
    }

    private static string GenerateExplanation(Career career, double overallScore, double techScore, double interestScore, List<SkillMatchDetail> matching, List<SkillMatchDetail> missing)
    {
        var matchedTechNames = matching.Where(m => m.Category == "Technical").Select(m => m.SkillName).Take(3).ToList();
        var matchedSoftNames = matching.Where(m => m.Category == "Soft").Select(m => m.SkillName).Take(2).ToList();

        string techSummary = matchedTechNames.Any()
            ? $"You possess core technical competencies in {string.Join(", ", matchedTechNames)}."
            : "You have foundational skills with opportunity to acquire domain-specific technical capabilities.";

        string interestSummary = interestScore >= 80
            ? $"Your stated interests strongly correlate with the {career.Category} sector."
            : $"Your profile attributes indicate positive potential in {career.Title} workflows.";

        string fitTier = overallScore switch
        {
            >= 85 => "Outstanding Match",
            >= 70 => "Strong Match",
            >= 55 => "Moderate Match with Growth Potential",
            _ => "Development Opportunity"
        };

        return $"{fitTier}: {techSummary} {interestSummary} With a {overallScore:F0}% weighted score, this pathway aligns closely with your educational background and work preferences.";
    }

    private static string GenerateImprovementSuggestions(Career career, List<SkillMatchDetail> missing)
    {
        var highPriority = missing.Where(m => m.Priority == "High" && m.IsRequired).Select(m => m.SkillName).ToList();
        if (highPriority.Any())
        {
            return $"Prioritize mastering {string.Join(", ", highPriority.Take(3))} through practical projects. Consult the career roadmap below to bridge these competencies.";
        }

        var anyMissing = missing.Select(m => m.SkillName).Take(3).ToList();
        if (anyMissing.Any())
        {
            return $"Enhance your marketability by gaining familiarity with {string.Join(", ", anyMissing)} and following structured milestones.";
        }

        return $"You have strong coverage of primary prerequisites. Deepen your mastery of advanced {career.Title} topics and build full-lifecycle portfolio projects.";
    }
}
