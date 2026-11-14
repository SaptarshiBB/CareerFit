using CareerFit.Data;
using CareerFit.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Services;

public class AdminService : IAdminService
{
    private readonly ApplicationDbContext _context;

    public AdminService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AdminStatsDto> GetAdminStatsAsync()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalAssessments = await _context.Assessments.CountAsync();
        var totalCareers = await _context.Careers.CountAsync();
        var totalSkills = await _context.Skills.CountAsync();

        var recentUsers = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .Take(6)
            .ToListAsync();

        var recentAssessments = await _context.Assessments
            .Include(a => a.User)
            .OrderByDescending(a => a.CreatedAt)
            .Take(6)
            .ToListAsync();

        // Top recommended careers
        var topRecs = await _context.Recommendations
            .Include(r => r.Career)
            .GroupBy(r => new { r.CareerId, r.Career!.Title, r.Career.Category })
            .Select(g => new RecommendedCareerStatDto
            {
                CareerTitle = g.Key.Title,
                Category = g.Key.Category,
                RecommendationCount = g.Count(),
                AverageScore = Math.Round(g.Average(r => r.OverallScore), 1)
            })
            .OrderByDescending(x => x.RecommendationCount)
            .Take(5)
            .ToListAsync();

        return new AdminStatsDto
        {
            TotalUsers = totalUsers,
            TotalAssessments = totalAssessments,
            TotalCareers = totalCareers,
            TotalSkills = totalSkills,
            TopRecommendedCareers = topRecs,
            RecentUsers = recentUsers,
            RecentAssessments = recentAssessments
        };
    }

    public async Task<List<ApplicationUser>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.CandidateProfile)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Career>> GetAllCareersAsync()
    {
        return await _context.Careers
            .Include(c => c.CareerSkills)
                .ThenInclude(cs => cs.Skill)
            .OrderBy(c => c.Category)
            .ThenBy(c => c.Title)
            .ToListAsync();
    }

    public async Task<Career?> GetCareerForEditAsync(int id)
    {
        return await _context.Careers
            .Include(c => c.CareerSkills)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task CreateCareerAsync(Career career, List<int> selectedSkillIds)
    {
        career.CreatedAt = DateTime.UtcNow;
        _context.Careers.Add(career);
        await _context.SaveChangesAsync();

        if (selectedSkillIds.Any())
        {
            foreach (var sId in selectedSkillIds)
            {
                _context.CareerSkills.Add(new CareerSkill
                {
                    CareerId = career.Id,
                    SkillId = sId,
                    IsRequired = true,
                    Priority = "High",
                    MinProficiency = "Intermediate"
                });
            }
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateCareerAsync(Career career, List<int> selectedSkillIds)
    {
        var existing = await _context.Careers
            .Include(c => c.CareerSkills)
            .FirstOrDefaultAsync(c => c.Id == career.Id);

        if (existing == null) return;

        existing.Title = career.Title;
        existing.Category = career.Category;
        existing.Summary = career.Summary;
        existing.Description = career.Description;
        existing.Responsibilities = career.Responsibilities;
        existing.RequiredEducation = career.RequiredEducation;
        existing.ExperienceLevel = career.ExperienceLevel;
        existing.ExperienceRequirements = career.ExperienceRequirements;
        existing.SuggestedLearningTopics = career.SuggestedLearningTopics;
        existing.SalaryMin = career.SalaryMin;
        existing.SalaryMax = career.SalaryMax;
        existing.GrowthOpportunities = career.GrowthOpportunities;
        existing.DemandLevel = career.DemandLevel;
        existing.WorkEnvironment = career.WorkEnvironment;
        existing.IsActive = career.IsActive;

        // Sync skills
        _context.CareerSkills.RemoveRange(existing.CareerSkills);
        foreach (var sId in selectedSkillIds)
        {
            _context.CareerSkills.Add(new CareerSkill
            {
                CareerId = existing.Id,
                SkillId = sId,
                IsRequired = true,
                Priority = "High",
                MinProficiency = "Intermediate"
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteCareerAsync(int id)
    {
        var career = await _context.Careers.FindAsync(id);
        if (career == null) return false;

        _context.Careers.Remove(career);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Skill>> GetAllSkillsAsync()
    {
        return await _context.Skills
            .Include(s => s.CareerSkills)
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }

    public async Task CreateSkillAsync(Skill skill)
    {
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteSkillAsync(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill == null) return false;

        _context.Skills.Remove(skill);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Question>> GetAllQuestionsAsync()
    {
        return await _context.Questions
            .Include(q => q.Options.OrderBy(o => o.OrderNumber))
            .OrderBy(q => q.OrderNumber)
            .ToListAsync();
    }

    public async Task CreateQuestionAsync(Question question)
    {
        _context.Questions.Add(question);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteQuestionAsync(int id)
    {
        var question = await _context.Questions.FindAsync(id);
        if (question == null) return false;

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();
        return true;
    }
}
