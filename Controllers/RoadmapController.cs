using System.Security.Claims;
using CareerFit.Data;
using CareerFit.Models;
using CareerFit.Services;
using CareerFit.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Controllers;

[Authorize]
public class RoadmapController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IRoadmapService _roadmapService;

    public RoadmapController(ApplicationDbContext context, IRoadmapService roadmapService)
    {
        _context = context;
        _roadmapService = roadmapService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var roadmaps = await _roadmapService.GetUserRoadmapsAsync(userId);
        return View(roadmaps);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var roadmap = await _roadmapService.GetRoadmapByIdAsync(id, userId);

        if (roadmap == null) return NotFound();

        var progressPct = await _roadmapService.CalculateRoadmapProgressAsync(id);
        var milestones = roadmap.Milestones.OrderBy(m => m.OrderNumber).ToList();

        var viewModel = new RoadmapDetailViewModel
        {
            Roadmap = roadmap,
            ProgressPercentage = progressPct,
            TotalCount = milestones.Count,
            CompletedCount = milestones.Count(m => m.IsCompleted),
            BeginnerMilestones = milestones.Where(m => m.Stage == "Beginner").ToList(),
            IntermediateMilestones = milestones.Where(m => m.Stage == "Intermediate").ToList(),
            AdvancedMilestones = milestones.Where(m => m.Stage == "Advanced").ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(int careerId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var roadmap = await _roadmapService.GetOrCreateRoadmapForCareerAsync(userId, careerId);

        TempData["SuccessMessage"] = $"Your structured learning roadmap for {roadmap.TargetRole} is ready!";
        return RedirectToAction(nameof(Details), new { id = roadmap.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleMilestone(int milestoneId, int roadmapId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var success = await _roadmapService.ToggleMilestoneCompletionAsync(milestoneId, userId);

        if (!success)
        {
            TempData["ErrorMessage"] = "Unable to update milestone status.";
        }

        return RedirectToAction(nameof(Details), new { id = roadmapId });
    }
}
