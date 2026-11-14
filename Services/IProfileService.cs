using CareerFit.Models;

namespace CareerFit.Services;

public interface IProfileService
{
    Task<CandidateProfile?> GetProfileByUserIdAsync(string userId);
    Task<CandidateProfile> GetOrCreateProfileAsync(string userId, string email, string fullName);
    Task UpdateProfileAsync(CandidateProfile profile);
    Task<int> CalculateCompletionPercentageAsync(string userId);
    Task AddUserSkillAsync(int profileId, int skillId, string proficiency, int yearsExp);
    Task RemoveUserSkillAsync(int profileId, int userSkillId);
    Task<List<Skill>> GetAllAvailableSkillsAsync();
}
