using CareerFit.Data;
using CareerFit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Controllers;

public class HomeIndexViewModel
{
    public List<CareerCategory> Categories { get; set; } = new();
    public List<Career> FeaturedCareers { get; set; } = new();
    public int TotalCareersCount { get; set; }
    public int TotalAssessmentsCount { get; set; }
}

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.CareerCategories
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        var featuredCareers = await _context.Careers
            .Where(c => c.IsActive)
            .OrderByDescending(c => c.DemandLevel == "Very High")
            .ThenBy(c => c.Title)
            .Take(6)
            .ToListAsync();

        var totalCareers = await _context.Careers.CountAsync();
        var totalAssessments = await _context.Assessments.CountAsync(a => a.IsCompleted);

        var viewModel = new HomeIndexViewModel
        {
            Categories = categories,
            FeaturedCareers = featuredCareers,
            TotalCareersCount = totalCareers,
            TotalAssessmentsCount = totalAssessments
        };

        return View(viewModel);
    }

    public IActionResult HowItWorks()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
