using System.Security.Claims;
using CareerFit.Data;
using CareerFit.Services;
using CareerFit.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Controllers;

[Authorize]
public class SkillGapController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ISkillGapService _skillGapService;
    private readonly ICareerMatchingService _matchingService;

    public SkillGapController(
        ApplicationDbContext context,
        ISkillGapService skillGapService,
        ICareerMatchingService matchingService)
    {
        _context = context;
        _skillGapService = skillGapService;
        _matchingService = matchingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? careerId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var availableCareers = await _context.Careers
            .Where(c => c.IsActive)
            .OrderBy(c => c.Title)
            .ToListAsync();

        if (!availableCareers.Any())
        {
            return View(new SkillGapViewModel { AvailableCareers = availableCareers });
        }

        int targetCareerId = careerId ?? availableCareers.First().Id;

        // If no careerId passed, prefer top recommended career if user has one
        if (!careerId.HasValue)
        {
            try
            {
                var recs = await _matchingService.CalculateRecommendationsAsync(userId);
                if (recs.Any())
                {
                    targetCareerId = recs.First().Career.Id;
                }
            }
            catch
            {
                // Fallback to first career
            }
        }

        var analysis = await _skillGapService.AnalyzeSkillGapAsync(userId, targetCareerId);
        bool hasProfileOrAssessment = await _matchingService.HasSufficientDataAsync(userId);

        var viewModel = new SkillGapViewModel
        {
            SelectedCareerId = targetCareerId,
            AvailableCareers = availableCareers,
            Analysis = analysis,
            HasProfileOrAssessment = hasProfileOrAssessment
        };

        return View(viewModel);
    }
}
