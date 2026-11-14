using CareerFit.Models;
using CareerFit.Services;

namespace CareerFit.ViewModels;

public class SkillGapViewModel
{
    public int SelectedCareerId { get; set; }
    public List<Career> AvailableCareers { get; set; } = new();
    public SkillGapAnalysisResult? Analysis { get; set; }
    public bool HasProfileOrAssessment { get; set; }
}
