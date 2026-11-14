using CareerFit.Data;
using CareerFit.Models;
using CareerFit.Services;
using CareerFit.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAdminService _adminService;
    private readonly IProfileService _profileService;

    public AdminController(
        ApplicationDbContext context,
        IAdminService adminService,
        IProfileService profileService)
    {
        _context = context;
        _adminService = adminService;
        _profileService = profileService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var stats = await _adminService.GetAdminStatsAsync();
        var viewModel = new AdminDashboardViewModel { Stats = stats };
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Users()
    {
        var users = await _adminService.GetAllUsersAsync();
        return View(users);
    }

    [HttpGet]
    public async Task<IActionResult> Careers()
    {
        var careers = await _adminService.GetAllCareersAsync();
        return View(careers);
    }

    [HttpGet]
    public async Task<IActionResult> CareerCreate()
    {
        var skills = await _adminService.GetAllSkillsAsync();
        var categories = await _context.CareerCategories.Select(c => c.Name).ToListAsync();

        var viewModel = new CareerEditViewModel
        {
            AvailableSkills = skills,
            AvailableCategories = categories
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CareerCreate(CareerEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableSkills = await _adminService.GetAllSkillsAsync();
            model.AvailableCategories = await _context.CareerCategories.Select(c => c.Name).ToListAsync();
            return View(model);
        }

        var cat = await _context.CareerCategories.FirstOrDefaultAsync(c => c.Name == model.Category);

        var career = new Career
        {
            Title = model.Title,
            Category = model.Category,
            CategoryId = cat?.Id,
            Summary = model.Summary,
            Description = model.Description,
            Responsibilities = model.Responsibilities,
            RequiredEducation = model.RequiredEducation,
            ExperienceLevel = model.ExperienceLevel,
            ExperienceRequirements = model.ExperienceRequirements,
            SuggestedLearningTopics = model.SuggestedLearningTopics,
            SalaryMin = model.SalaryMin,
            SalaryMax = model.SalaryMax,
            GrowthOpportunities = model.GrowthOpportunities,
            DemandLevel = model.DemandLevel,
            WorkEnvironment = model.WorkEnvironment,
            IsActive = model.IsActive
        };

        await _adminService.CreateCareerAsync(career, model.SelectedSkillIds);
        TempData["SuccessMessage"] = $"Career profile '{career.Title}' was successfully created.";
        return RedirectToAction(nameof(Careers));
    }

    [HttpGet]
    public async Task<IActionResult> CareerEdit(int id)
    {
        var career = await _adminService.GetCareerForEditAsync(id);
        if (career == null) return NotFound();

        var skills = await _adminService.GetAllSkillsAsync();
        var categories = await _context.CareerCategories.Select(c => c.Name).ToListAsync();

        var viewModel = new CareerEditViewModel
        {
            Id = career.Id,
            Title = career.Title,
            Category = career.Category,
            Summary = career.Summary,
            Description = career.Description,
            Responsibilities = career.Responsibilities,
            RequiredEducation = career.RequiredEducation,
            ExperienceLevel = career.ExperienceLevel,
            ExperienceRequirements = career.ExperienceRequirements,
            SuggestedLearningTopics = career.SuggestedLearningTopics,
            SalaryMin = career.SalaryMin,
            SalaryMax = career.SalaryMax,
            GrowthOpportunities = career.GrowthOpportunities,
            DemandLevel = career.DemandLevel,
            WorkEnvironment = career.WorkEnvironment,
            IsActive = career.IsActive,
            SelectedSkillIds = career.CareerSkills.Select(cs => cs.SkillId).ToList(),
            AvailableSkills = skills,
            AvailableCategories = categories
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CareerEdit(CareerEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableSkills = await _adminService.GetAllSkillsAsync();
            model.AvailableCategories = await _context.CareerCategories.Select(c => c.Name).ToListAsync();
            return View(model);
        }

        var career = new Career
        {
            Id = model.Id,
            Title = model.Title,
            Category = model.Category,
            Summary = model.Summary,
            Description = model.Description,
            Responsibilities = model.Responsibilities,
            RequiredEducation = model.RequiredEducation,
            ExperienceLevel = model.ExperienceLevel,
            ExperienceRequirements = model.ExperienceRequirements,
            SuggestedLearningTopics = model.SuggestedLearningTopics,
            SalaryMin = model.SalaryMin,
            SalaryMax = model.SalaryMax,
            GrowthOpportunities = model.GrowthOpportunities,
            DemandLevel = model.DemandLevel,
            WorkEnvironment = model.WorkEnvironment,
            IsActive = model.IsActive
        };

        await _adminService.UpdateCareerAsync(career, model.SelectedSkillIds);
        TempData["SuccessMessage"] = $"Career profile '{career.Title}' was updated successfully.";
        return RedirectToAction(nameof(Careers));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CareerDelete(int id)
    {
        var result = await _adminService.DeleteCareerAsync(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Career profile deleted safely.";
        }
        else
        {
            TempData["ErrorMessage"] = "Could not delete career.";
        }
        return RedirectToAction(nameof(Careers));
    }

    [HttpGet]
    public async Task<IActionResult> Skills()
    {
        var skills = await _adminService.GetAllSkillsAsync();
        return View(skills);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SkillCreate(SkillEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Skill name and category are required.";
            return RedirectToAction(nameof(Skills));
        }

        var skill = new Skill
        {
            Name = model.Name,
            Category = model.Category,
            Description = model.Description
        };

        await _adminService.CreateSkillAsync(skill);
        TempData["SuccessMessage"] = $"Skill '{skill.Name}' created successfully.";
        return RedirectToAction(nameof(Skills));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SkillDelete(int id)
    {
        var result = await _adminService.DeleteSkillAsync(id);
        if (result)
        {
            TempData["SuccessMessage"] = "Skill deleted successfully.";
        }
        else
        {
            TempData["ErrorMessage"] = "Could not delete skill.";
        }
        return RedirectToAction(nameof(Skills));
    }

    [HttpGet]
    public async Task<IActionResult> Questions()
    {
        var questions = await _adminService.GetAllQuestionsAsync();
        return View(questions);
    }
}
