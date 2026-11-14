using CareerFit.Models;
using CareerFit.Services;

namespace CareerFit.ViewModels;

public class AssessmentStepAnswerInput
{
    public int QuestionId { get; set; }
    public string SelectedOptionIds { get; set; } = string.Empty;
    public string SelectedValues { get; set; } = string.Empty;
    public double? NumericScore { get; set; }
}

public class AssessmentWizardViewModel
{
    public int AssessmentId { get; set; }
    public int CurrentStepIndex { get; set; } // 0 to 7 (for 8 sections: A to H)
    public int TotalSteps { get; set; } = 8;
    public string SectionKey { get; set; } = string.Empty;
    public string SectionTitle { get; set; } = string.Empty;
    public string SectionLetter { get; set; } = "A";
    public int ProgressPercentage => (int)Math.Round((double)(CurrentStepIndex + 1) / TotalSteps * 100);

    public List<Question> SectionQuestions { get; set; } = new();

    // Map of QuestionId -> Answer
    public Dictionary<int, AssessmentStepAnswerInput> Answers { get; set; } = new();

    public bool IsFirstStep => CurrentStepIndex == 0;
    public bool IsLastStep => CurrentStepIndex == TotalSteps - 1;
}

public class AssessmentResultViewModel
{
    public Assessment Assessment { get; set; } = null!;
    public List<CareerMatchResult> TopRecommendations { get; set; } = new();
    public List<CareerMatchResult> AllRecommendations { get; set; } = new();
    public CandidateProfile? Profile { get; set; }
}
