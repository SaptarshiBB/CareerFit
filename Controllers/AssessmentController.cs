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
public class AssessmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ICareerMatchingService _matchingService;

    // The 8 section keys in order (A through H)
    private static readonly string[] SectionKeys = new[]
    {
        "Education",        // Step 0 - Section A
        "TechnicalSkills",  // Step 1 - Section B
        "SoftSkills",       // Step 2 - Section C
        "Interests",        // Step 3 - Section D
        "ProblemSolving",   // Step 4 - Section E
        "WorkEnvironment",  // Step 5 - Section F
        "CareerGoals",      // Step 6 - Section G
        "ExperienceLevel"   // Step 7 - Section H
    };

    private static readonly Dictionary<string, (string Letter, string Title)> SectionMetadata = new()
    {
        ["Education"] = ("A", "Educational Background"),
        ["TechnicalSkills"] = ("B", "Technical Skills"),
        ["SoftSkills"] = ("C", "Soft Skills"),
        ["Interests"] = ("D", "Personal Interests"),
        ["ProblemSolving"] = ("E", "Problem-Solving Abilities"),
        ["WorkEnvironment"] = ("F", "Preferred Work Environment"),
        ["CareerGoals"] = ("G", "Career Goals"),
        ["ExperienceLevel"] = ("H", "Experience Level")
    };

    public AssessmentController(ApplicationDbContext context, ICareerMatchingService matchingService)
    {
        _context = context;
        _matchingService = matchingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Check if there is an in-progress assessment
        var activeAssessment = await _context.Assessments
            .Where(a => a.UserId == userId && !a.IsCompleted)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync();

        if (activeAssessment == null)
        {
            activeAssessment = new Assessment
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false,
                SummaryTitle = "Comprehensive Career Assessment"
            };
            _context.Assessments.Add(activeAssessment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Step), new { id = activeAssessment.Id, step = 0 });
    }

    [HttpGet]
    public async Task<IActionResult> Retake()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Create a new assessment
        var newAssessment = new Assessment
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false,
            SummaryTitle = "Comprehensive Career Assessment"
        };
        _context.Assessments.Add(newAssessment);
        await _context.SaveChangesAsync();

        TempData["InfoMessage"] = "A new assessment session has begun. Please respond to the questions carefully.";
        return RedirectToAction(nameof(Step), new { id = newAssessment.Id, step = 0 });
    }

    [HttpGet]
    public async Task<IActionResult> Step(int id, int step = 0)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var assessment = await _context.Assessments
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (assessment == null) return NotFound();

        if (step < 0) step = 0;
        if (step >= SectionKeys.Length)
        {
            return RedirectToAction(nameof(Finish), new { id = assessment.Id });
        }

        var currentSectionKey = SectionKeys[step];
        var meta = SectionMetadata[currentSectionKey];

        var questions = await _context.Questions
            .Where(q => q.SectionKey == currentSectionKey)
            .Include(q => q.Options.OrderBy(o => o.OrderNumber))
            .OrderBy(q => q.OrderNumber)
            .ToListAsync();

        var answersMap = new Dictionary<int, AssessmentStepAnswerInput>();
        foreach (var q in questions)
        {
            var existingAns = assessment.Answers.FirstOrDefault(a => a.QuestionId == q.Id);
            answersMap[q.Id] = new AssessmentStepAnswerInput
            {
                QuestionId = q.Id,
                SelectedOptionIds = existingAns?.SelectedOptionIds ?? string.Empty,
                SelectedValues = existingAns?.SelectedValues ?? string.Empty,
                NumericScore = existingAns?.NumericScore
            };
        }

        var viewModel = new AssessmentWizardViewModel
        {
            AssessmentId = assessment.Id,
            CurrentStepIndex = step,
            TotalSteps = SectionKeys.Length,
            SectionKey = currentSectionKey,
            SectionTitle = $"Section {meta.Letter}: {meta.Title}",
            SectionLetter = meta.Letter,
            SectionQuestions = questions,
            Answers = answersMap
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Step(int assessmentId, int stepIndex, string direction, IFormCollection form)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var assessment = await _context.Assessments
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == assessmentId && a.UserId == userId);

        if (assessment == null) return NotFound();

        var currentSectionKey = SectionKeys[stepIndex];
        var questions = await _context.Questions
            .Where(q => q.SectionKey == currentSectionKey)
            .Include(q => q.Options)
            .ToListAsync();

        // Process submitted answers for this step
        foreach (var q in questions)
        {
            string optionKey = $"q_{q.Id}";
            var selectedValuesList = form[optionKey].ToList();

            if (selectedValuesList.Any())
            {
                var existingAns = assessment.Answers.FirstOrDefault(a => a.QuestionId == q.Id);
                if (existingAns == null)
                {
                    existingAns = new AssessmentAnswer
                    {
                        AssessmentId = assessment.Id,
                        QuestionId = q.Id,
                        AnsweredAt = DateTime.UtcNow
                    };
                    assessment.Answers.Add(existingAns);
                }

                // If RatingScale, parse numeric value
                if (q.QuestionType == "RatingScale")
                {
                    if (double.TryParse(selectedValuesList.First(), out double numScore))
                    {
                        existingAns.NumericScore = numScore;
                        existingAns.SelectedValues = selectedValuesList.First();
                        existingAns.SelectedOptionIds = selectedValuesList.First();
                    }
                }
                else
                {
                    // Multi-select or single-choice
                    var matchingOptions = q.Options.Where(o => selectedValuesList.Contains(o.OptionValue)).ToList();
                    existingAns.SelectedOptionIds = string.Join(",", matchingOptions.Select(o => o.Id));
                    existingAns.SelectedValues = string.Join(", ", matchingOptions.Select(o => o.OptionValue));
                }
            }
        }

        await _context.SaveChangesAsync();

        // Handle navigation direction
        if (direction == "prev")
        {
            int prevStep = Math.Max(0, stepIndex - 1);
            return RedirectToAction(nameof(Step), new { id = assessmentId, step = prevStep });
        }
        else // "next" or "submit"
        {
            if (stepIndex >= SectionKeys.Length - 1)
            {
                return RedirectToAction(nameof(Finish), new { id = assessmentId });
            }
            return RedirectToAction(nameof(Step), new { id = assessmentId, step = stepIndex + 1 });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Finish(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var assessment = await _context.Assessments
            .Include(a => a.Answers)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (assessment == null) return NotFound();

        assessment.IsCompleted = true;
        assessment.CompletedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Calculate recommendations using the weighted algorithm
        var recommendations = await _matchingService.CalculateRecommendationsAsync(userId, assessment.Id);

        TempData["SuccessMessage"] = "Assessment successfully completed! Here are your personalized career recommendations.";
        return RedirectToAction(nameof(Results), new { id = assessment.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Results(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var assessment = await _context.Assessments
            .Include(a => a.Recommendations)
                .ThenInclude(r => r.Career)
            .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);

        if (assessment == null) return NotFound();

        var profile = await _context.CandidateProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        var recommendations = await _matchingService.CalculateRecommendationsAsync(userId, assessment.Id);

        var viewModel = new AssessmentResultViewModel
        {
            Assessment = assessment,
            TopRecommendations = recommendations.Take(5).ToList(),
            AllRecommendations = recommendations,
            Profile = profile
        };

        return View(viewModel);
    }
}
