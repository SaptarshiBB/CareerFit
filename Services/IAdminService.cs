using CareerFit.Models;

namespace CareerFit.Services;

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalAssessments { get; set; }
    public int TotalCareers { get; set; }
    public int TotalSkills { get; set; }
    public List<RecommendedCareerStatDto> TopRecommendedCareers { get; set; } = new();
    public List<ApplicationUser> RecentUsers { get; set; } = new();
    public List<Assessment> RecentAssessments { get; set; } = new();
}

public class RecommendedCareerStatDto
{
    public string CareerTitle { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public int RecommendationCount { get; set; }
    public double AverageScore { get; set; }
}

public interface IAdminService
{
    Task<AdminStatsDto> GetAdminStatsAsync();
    Task<List<ApplicationUser>> GetAllUsersAsync();
    Task<List<Career>> GetAllCareersAsync();
    Task<Career?> GetCareerForEditAsync(int id);
    Task CreateCareerAsync(Career career, List<int> selectedSkillIds);
    Task UpdateCareerAsync(Career career, List<int> selectedSkillIds);
    Task<bool> DeleteCareerAsync(int id);
    Task<List<Skill>> GetAllSkillsAsync();
    Task CreateSkillAsync(Skill skill);
    Task<bool> DeleteSkillAsync(int id);
    Task<List<Question>> GetAllQuestionsAsync();
    Task CreateQuestionAsync(Question question);
    Task<bool> DeleteQuestionAsync(int id);
}
