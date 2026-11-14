using CareerFit.Data;
using CareerFit.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Services;

public class ProfileService : IProfileService
{
    private readonly ApplicationDbContext _context;

    public ProfileService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CandidateProfile?> GetProfileByUserIdAsync(string userId)
    {
        return await _context.CandidateProfiles
            .Include(p => p.UserSkills)
                .ThenInclude(us => us.Skill)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<CandidateProfile> GetOrCreateProfileAsync(string userId, string email, string fullName)
    {
        var profile = await GetProfileByUserIdAsync(userId);
        if (profile != null) return profile;

        profile = new CandidateProfile
        {
            UserId = userId,
            Email = email,
            FullName = fullName,
            HighestEducation = "Bachelor's Degree",
            ExperienceLevel = "Entry-Level",
            PreferredWorkEnvironment = "Hybrid",
            PreferredWorkDomain = "Information Technology",
            UpdatedAt = DateTime.UtcNow
        };

        _context.CandidateProfiles.Add(profile);
        await _context.SaveChangesAsync();

        return profile;
    }

    public async Task UpdateProfileAsync(CandidateProfile profile)
    {
        profile.UpdatedAt = DateTime.UtcNow;
        _context.CandidateProfiles.Update(profile);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CalculateCompletionPercentageAsync(string userId)
    {
        var profile = await _context.CandidateProfiles
            .Include(p => p.UserSkills)
            .FirstOrDefaultAsync(p => p.UserId == userId);

        if (profile == null) return 0;

        int score = 0;

        // 1. Personal & Contact (20 points)
        if (!string.IsNullOrWhiteSpace(profile.FullName)) score += 10;
        if (!string.IsNullOrWhiteSpace(profile.PhoneNumber)) score += 10;

        // 2. Education details (25 points)
        if (!string.IsNullOrWhiteSpace(profile.HighestEducation)) score += 10;
        if (!string.IsNullOrWhiteSpace(profile.Degree)) score += 10;
        if (profile.GraduationYear.HasValue) score += 5;

        // 3. Work Experience & Level (20 points)
        if (!string.IsNullOrWhiteSpace(profile.ExperienceLevel)) score += 10;
        if (!string.IsNullOrWhiteSpace(profile.WorkExperience)) score += 10;

        // 4. Skills possessed (20 points)
        if (profile.UserSkills.Count >= 3) score += 20;
        else if (profile.UserSkills.Any()) score += 10;

        // 5. Interests & Preferences (15 points)
        if (!string.IsNullOrWhiteSpace(profile.CareerInterests)) score += 10;
        if (!string.IsNullOrWhiteSpace(profile.PreferredWorkEnvironment)) score += 5;

        return Math.Min(100, score);
    }

    public async Task AddUserSkillAsync(int profileId, int skillId, string proficiency, int yearsExp)
    {
        var existing = await _context.UserSkills
            .FirstOrDefaultAsync(us => us.CandidateProfileId == profileId && us.SkillId == skillId);

        if (existing != null)
        {
            existing.ProficiencyLevel = proficiency;
            existing.YearsOfExperience = yearsExp;
        }
        else
        {
            _context.UserSkills.Add(new UserSkill
            {
                CandidateProfileId = profileId,
                SkillId = skillId,
                ProficiencyLevel = proficiency,
                YearsOfExperience = yearsExp
            });
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserSkillAsync(int profileId, int userSkillId)
    {
        var skill = await _context.UserSkills
            .FirstOrDefaultAsync(us => us.Id == userSkillId && us.CandidateProfileId == profileId);

        if (skill != null)
        {
            _context.UserSkills.Remove(skill);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Skill>> GetAllAvailableSkillsAsync()
    {
        return await _context.Skills
            .OrderBy(s => s.Category)
            .ThenBy(s => s.Name)
            .ToListAsync();
    }
}
