using CareerFit.Models;

namespace CareerFit.Services;

public interface IRoadmapService
{
    Task<Roadmap> GetOrCreateRoadmapForCareerAsync(string userId, int careerId);
    Task<List<Roadmap>> GetUserRoadmapsAsync(string userId);
    Task<Roadmap?> GetRoadmapByIdAsync(int roadmapId, string userId);
    Task<bool> ToggleMilestoneCompletionAsync(int milestoneId, string userId);
    Task<double> CalculateRoadmapProgressAsync(int roadmapId);
}
