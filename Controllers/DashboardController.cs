using System.Security.Claims;
using CareerFit.Data;
using CareerFit.Models;
using CareerFit.Services;
using CareerFit.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProfileService _profileService;
    private readonly ICareerMatchingService _matchingService;
    private readonly ISkillGapService _skillGapService;
    private readonly IRoadmapService _roadmapService;

    public DashboardController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IProfileService profileService,
        ICareerMatchingService matchingService,
        ISkillGapService skillGapService,
        IRoadmapService roadmapService)
    {
        _context = context;
        _userManager = userManager;
        _profileService = profileService;
        _matchingService = matchingService;
        _skillGapService = skillGapService;
        _roadmapService = roadmapService;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return RedirectToAction("Login", "Account");

        var profile = await _profileService.GetProfileByUserIdAsync(userId);
        var completionPct = await _profileService.CalculateCompletionPercentageAsync(userId);

        var latestAssessment = await _context.Assessments
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CompletedAt ?? a.CreatedAt)
            .FirstOrDefaultAsync();

        var assessmentHistory = await _context.Assessments
            .Where(a => a.UserId == userId && a.IsCompleted)
            .OrderByDescending(a => a.CompletedAt)
            .Take(5)
            .ToListAsync();

        var savedCareers = await _context.SavedCareers
            .Include(sc => sc.Career)
            .Where(sc => sc.UserId == userId)
            .OrderByDescending(sc => sc.SavedAt)
            .ToListAsync();

        var roadmaps = await _roadmapService.GetUserRoadmapsAsync(userId);

        // Calculate average roadmap progress
        double totalRoadmapProg = 0;
        foreach (var rm in roadmaps)
        {
            totalRoadmapProg += await _roadmapService.CalculateRoadmapProgressAsync(rm.Id);
        }
        double avgRoadmapProg = roadmaps.Any() ? Math.Round(totalRoadmapProg / roadmaps.Count, 1) : 0;

        // Recommendations
        List<CareerMatchResult> recs = new();
        CareerMatchResult? topRec = null;
        List<SkillGapItem> keyMissing = new();

        bool hasSufficient = await _matchingService.HasSufficientDataAsync(userId);
        if (hasSufficient)
        {
            try
            {
                recs = await _matchingService.CalculateRecommendationsAsync(userId);
                topRec = recs.FirstOrDefault();

                if (topRec != null)
                {
                    var gap = await _skillGapService.AnalyzeSkillGapAsync(userId, topRec.Career.Id);
                    keyMissing = gap.MissingSkills.Take(5).ToList();
                }
            }
            catch
            {
                // Fallback gracefully
            }
        }

        var viewModel = new UserDashboardViewModel
        {
            User = user,
            Profile = profile,
            ProfileCompletionPercentage = completionPct,
            LatestAssessment = latestAssessment,
            TopRecommendation = topRec,
            Top5Recommendations = recs.Take(5).ToList(),
            KeyMissingSkills = keyMissing,
            ActiveRoadmaps = roadmaps,
            AverageRoadmapProgress = avgRoadmapProg,
            SavedCareers = savedCareers,
            AssessmentHistory = assessmentHistory
        };

        return View(viewModel);
    }
}
