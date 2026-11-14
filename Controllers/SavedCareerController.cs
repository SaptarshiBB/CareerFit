using System.Security.Claims;
using CareerFit.Data;
using CareerFit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Controllers;

[Authorize]
public class SavedCareerController : Controller
{
    private readonly ApplicationDbContext _context;

    public SavedCareerController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var savedCareers = await _context.SavedCareers
            .Include(sc => sc.Career)
                .ThenInclude(c => c!.CareerSkills)
                    .ThenInclude(cs => cs.Skill)
            .Where(sc => sc.UserId == userId)
            .OrderByDescending(sc => sc.SavedAt)
            .ToListAsync();

        return View(savedCareers);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int careerId, string? returnUrl = null)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var existing = await _context.SavedCareers
            .FirstOrDefaultAsync(sc => sc.UserId == userId && sc.CareerId == careerId);

        if (existing != null)
        {
            _context.SavedCareers.Remove(existing);
            await _context.SaveChangesAsync();
            TempData["InfoMessage"] = "Career removed from your saved bookmarks.";
        }
        else
        {
            var career = await _context.Careers.FindAsync(careerId);
            if (career != null)
            {
                _context.SavedCareers.Add(new SavedCareer
                {
                    UserId = userId,
                    CareerId = careerId,
                    SavedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"'{career.Title}' added to your saved careers!";
            }
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var existing = await _context.SavedCareers
            .FirstOrDefaultAsync(sc => sc.Id == id && sc.UserId == userId);

        if (existing != null)
        {
            _context.SavedCareers.Remove(existing);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Career bookmark removed.";
        }

        return RedirectToAction(nameof(Index));
    }
}
