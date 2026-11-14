using System.Security.Claims;
using CareerFit.Data;
using CareerFit.Services;
using CareerFit.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Controllers;

[Authorize]
public class RecommendationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICareerMatchingService _matchingService;

    public RecommendationController(ApplicationDbContext context, ICareerMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        bool hasSufficientData = await _matchingService.HasSufficientDataAsync(userId);
        if (!hasSufficientData)
        {
            ViewBag.HasSufficientData = false;
            return View(new List<CareerMatchResult>());
        }

        ViewBag.HasSufficientData = true;
        var recommendations = await _matchingService.CalculateRecommendationsAsync(userId);
        return View(recommendations);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int careerId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var match = await _matchingService.CalculateSingleCareerMatchAsync(userId, careerId);
        if (match == null) return NotFound();

        return View(match);
    }
}
