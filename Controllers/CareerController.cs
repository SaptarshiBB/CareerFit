using System.Security.Claims;
using CareerFit.Data;
using CareerFit.Models;
using CareerFit.Services;
using CareerFit.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Controllers;

public class CareerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICareerMatchingService _matchingService;

    public CareerController(ApplicationDbContext context, ICareerMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, string? category, string? experience, string? sortBy)
    {
        var query = _context.Careers
            .Where(c => c.IsActive)
            .Include(c => c.CareerSkills)
                .ThenInclude(cs => cs.Skill)
            .AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(term) ||
                                     c.Summary.ToLower().Contains(term) ||
                                     c.Category.ToLower().Contains(term) ||
                                     c.CareerSkills.Any(cs => cs.Skill != null && cs.Skill.Name.ToLower().Contains(term)));
        }

        // Category filter
        if (!string.IsNullOrWhiteSpace(category) && category != "All")
        {
            query = query.Where(c => c.Category == category);
        }

        // Experience filter
        if (!string.IsNullOrWhiteSpace(experience) && experience != "All")
        {
            query = query.Where(c => c.ExperienceLevel == experience);
        }

        // Sorting
        query = sortBy switch
        {
            "title_desc" => query.OrderByDescending(c => c.Title),
            "salary_high" => query.OrderByDescending(c => c.SalaryMax),
            "salary_low" => query.OrderBy(c => c.SalaryMin),
            "demand" => query.OrderByDescending(c => c.DemandLevel == "Very High").ThenBy(c => c.Title),
            _ => query.OrderBy(c => c.Title)
        };

        var careers = await query.ToListAsync();

        var categories = await _context.Careers
            .Where(c => c.IsActive)
            .Select(c => c.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        var expLevels = new List<string> { "Entry-Level", "Mid-Level", "Senior" };

        var viewModel = new CareerIndexViewModel
        {
            Careers = careers,
            Categories = categories,
            ExperienceLevels = expLevels,
            SearchTerm = searchTerm,
            SelectedCategory = category,
            SelectedExperience = experience,
            SortBy = sortBy
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var career = await _context.Careers
            .Include(c => c.CareerSkills)
                .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (career == null) return NotFound();

        var relatedCareers = await _context.Careers
            .Where(c => c.IsActive && c.Id != id && c.Category == career.Category)
            .Take(3)
            .ToListAsync();

        bool isBookmarked = false;
        bool hasRoadmap = false;
        int? existingRoadmapId = null;
        CareerMatchResult? personalMatch = null;

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            isBookmarked = await _context.SavedCareers.AnyAsync(sc => sc.UserId == userId && sc.CareerId == id);

            var roadmap = await _context.Roadmaps.FirstOrDefaultAsync(r => r.UserId == userId && r.CareerId == id);
            if (roadmap != null)
            {
                hasRoadmap = true;
                existingRoadmapId = roadmap.Id;
            }

            try
            {
                personalMatch = await _matchingService.CalculateSingleCareerMatchAsync(userId, id);
            }
            catch
            {
                // Graceful fallback if user has no profile or assessment yet
            }
        }

        var viewModel = new CareerDetailViewModel
        {
            Career = career,
            RelatedCareers = relatedCareers,
            IsBookmarked = isBookmarked,
            HasRoadmap = hasRoadmap,
            ExistingRoadmapId = existingRoadmapId,
            PersonalMatch = personalMatch
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Compare(int? career1Id, int? career2Id)
    {
        var allCareers = await _context.Careers
            .Where(c => c.IsActive)
            .OrderBy(c => c.Title)
            .ToListAsync();

        Career? c1 = null;
        Career? c2 = null;
        CareerMatchResult? m1 = null;
        CareerMatchResult? m2 = null;

        string? userId = User.Identity?.IsAuthenticated == true
            ? User.FindFirstValue(ClaimTypes.NameIdentifier)
            : null;

        if (career1Id.HasValue)
        {
            c1 = await _context.Careers
                .Include(c => c.CareerSkills)
                    .ThenInclude(cs => cs.Skill)
                .FirstOrDefaultAsync(c => c.Id == career1Id.Value);

            if (c1 != null && userId != null)
            {
                m1 = await _matchingService.CalculateSingleCareerMatchAsync(userId, c1.Id);
            }
        }

        if (career2Id.HasValue)
        {
            c2 = await _context.Careers
                .Include(c => c.CareerSkills)
                    .ThenInclude(cs => cs.Skill)
                .FirstOrDefaultAsync(c => c.Id == career2Id.Value);

            if (c2 != null && userId != null)
            {
                m2 = await _matchingService.CalculateSingleCareerMatchAsync(userId, c2.Id);
            }
        }

        var viewModel = new CareerCompareViewModel
        {
            AvailableCareers = allCareers,
            Career1Id = career1Id,
            Career2Id = career2Id,
            Career1 = c1,
            Career2 = c2,
            Match1 = m1,
            Match2 = m2
        };

        return View(viewModel);
    }
}
