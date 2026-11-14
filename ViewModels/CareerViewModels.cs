using CareerFit.Models;
using CareerFit.Services;

namespace CareerFit.ViewModels;

public class CareerIndexViewModel
{
    public List<Career> Careers { get; set; } = new();
    public List<string> Categories { get; set; } = new();
    public List<string> ExperienceLevels { get; set; } = new();

    public string? SearchTerm { get; set; }
    public string? SelectedCategory { get; set; }
    public string? SelectedExperience { get; set; }
    public string? SortBy { get; set; } // "title_asc", "title_desc", "salary_high", "demand"

    public int TotalCount => Careers.Count;
}

public class CareerDetailViewModel
{
    public Career Career { get; set; } = null!;
    public List<Career> RelatedCareers { get; set; } = new();
    public bool IsBookmarked { get; set; }
    public bool HasRoadmap { get; set; }
    public int? ExistingRoadmapId { get; set; }
    public CareerMatchResult? PersonalMatch { get; set; }
}

public class CareerCompareViewModel
{
    public List<Career> AvailableCareers { get; set; } = new();
    public int? Career1Id { get; set; }
    public int? Career2Id { get; set; }
    public Career? Career1 { get; set; }
    public Career? Career2 { get; set; }
    public CareerMatchResult? Match1 { get; set; }
    public CareerMatchResult? Match2 { get; set; }
}
