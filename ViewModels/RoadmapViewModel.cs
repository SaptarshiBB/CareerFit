using CareerFit.Models;

namespace CareerFit.ViewModels;

public class RoadmapDetailViewModel
{
    public Roadmap Roadmap { get; set; } = null!;
    public double ProgressPercentage { get; set; }
    public int CompletedCount { get; set; }
    public int TotalCount { get; set; }

    public List<RoadmapMilestone> BeginnerMilestones { get; set; } = new();
    public List<RoadmapMilestone> IntermediateMilestones { get; set; } = new();
    public List<RoadmapMilestone> AdvancedMilestones { get; set; } = new();
}
