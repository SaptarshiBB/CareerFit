using System.Security.Claims;
using CareerFit.Models;
using CareerFit.Services;
using CareerFit.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CareerFit.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IProfileService _profileService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileController(IProfileService profileService, UserManager<ApplicationUser> userManager)
    {
        _profileService = profileService;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return RedirectToAction("Login", "Account");

        var profile = await _profileService.GetOrCreateProfileAsync(userId, user.Email!, user.FullName);
        var completionPct = await _profileService.CalculateCompletionPercentageAsync(userId);
        var availableSkills = await _profileService.GetAllAvailableSkillsAsync();

        ViewBag.AvailableSkills = availableSkills;

        var viewModel = new CandidateProfileViewModel
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            HighestEducation = profile.HighestEducation,
            Degree = profile.Degree,
            Specialization = profile.Specialization,
            GraduationYear = profile.GraduationYear,
            ExperienceLevel = profile.ExperienceLevel,
            YearsOfExperience = profile.YearsOfExperience,
            WorkExperience = profile.WorkExperience,
            CareerInterests = profile.CareerInterests,
            PreferredWorkDomain = profile.PreferredWorkDomain,
            PreferredWorkEnvironment = profile.PreferredWorkEnvironment,
            CompletionPercentage = completionPct,
            CurrentSkills = profile.UserSkills.ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return RedirectToAction("Login", "Account");

        var profile = await _profileService.GetOrCreateProfileAsync(userId, user.Email!, user.FullName);
        var completionPct = await _profileService.CalculateCompletionPercentageAsync(userId);
        var availableSkills = await _profileService.GetAllAvailableSkillsAsync();

        ViewBag.AvailableSkills = availableSkills;

        var viewModel = new CandidateProfileViewModel
        {
            Id = profile.Id,
            FullName = profile.FullName,
            Email = profile.Email,
            PhoneNumber = profile.PhoneNumber,
            HighestEducation = profile.HighestEducation,
            Degree = profile.Degree,
            Specialization = profile.Specialization,
            GraduationYear = profile.GraduationYear,
            ExperienceLevel = profile.ExperienceLevel,
            YearsOfExperience = profile.YearsOfExperience,
            WorkExperience = profile.WorkExperience,
            CareerInterests = profile.CareerInterests,
            PreferredWorkDomain = profile.PreferredWorkDomain,
            PreferredWorkEnvironment = profile.PreferredWorkEnvironment,
            CompletionPercentage = completionPct,
            CurrentSkills = profile.UserSkills.ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CandidateProfileViewModel model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _profileService.GetProfileByUserIdAsync(userId);
        if (profile == null) return NotFound();

        if (!ModelState.IsValid)
        {
            ViewBag.AvailableSkills = await _profileService.GetAllAvailableSkillsAsync();
            model.CurrentSkills = profile.UserSkills.ToList();
            model.CompletionPercentage = await _profileService.CalculateCompletionPercentageAsync(userId);
            return View(model);
        }

        profile.FullName = model.FullName;
        profile.PhoneNumber = model.PhoneNumber;
        profile.HighestEducation = model.HighestEducation;
        profile.Degree = model.Degree ?? string.Empty;
        profile.Specialization = model.Specialization ?? string.Empty;
        profile.GraduationYear = model.GraduationYear;
        profile.ExperienceLevel = model.ExperienceLevel;
        profile.YearsOfExperience = model.YearsOfExperience;
        profile.WorkExperience = model.WorkExperience;
        profile.CareerInterests = model.CareerInterests ?? string.Empty;
        profile.PreferredWorkDomain = model.PreferredWorkDomain;
        profile.PreferredWorkEnvironment = model.PreferredWorkEnvironment;

        await _profileService.UpdateProfileAsync(profile);

        // Also update Identity user full name
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null && user.FullName != model.FullName)
        {
            user.FullName = model.FullName;
            await _userManager.UpdateAsync(user);
        }

        TempData["SuccessMessage"] = "Your candidate profile has been updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddSkill(int skillId, string proficiencyLevel, int yearsOfExperience)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _profileService.GetProfileByUserIdAsync(userId);
        if (profile == null) return NotFound();

        await _profileService.AddUserSkillAsync(profile.Id, skillId, proficiencyLevel, yearsOfExperience);
        TempData["SuccessMessage"] = "Skill added to your profile.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveSkill(int userSkillId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profile = await _profileService.GetProfileByUserIdAsync(userId);
        if (profile == null) return NotFound();

        await _profileService.RemoveUserSkillAsync(profile.Id, userSkillId);
        TempData["SuccessMessage"] = "Skill removed from your profile.";
        return RedirectToAction(nameof(Index));
    }
}
