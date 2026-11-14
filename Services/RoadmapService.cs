using CareerFit.Data;
using CareerFit.Models;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Services;

public class RoadmapService : IRoadmapService
{
    private readonly ApplicationDbContext _context;

    public RoadmapService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Roadmap> GetOrCreateRoadmapForCareerAsync(string userId, int careerId)
    {
        var existing = await _context.Roadmaps
            .Include(r => r.Milestones.OrderBy(m => m.OrderNumber))
            .Include(r => r.Career)
            .FirstOrDefaultAsync(r => r.UserId == userId && r.CareerId == careerId);

        if (existing != null)
            return existing;

        var career = await _context.Careers
            .Include(c => c.CareerSkills)
                .ThenInclude(cs => cs.Skill)
            .FirstOrDefaultAsync(c => c.Id == careerId)
            ?? throw new ArgumentException($"Career with ID {careerId} not found");

        var roadmap = new Roadmap
        {
            UserId = userId,
            CareerId = careerId,
            TargetRole = career.Title,
            Summary = $"Structured professional mastery roadmap for becoming a qualified {career.Title}.",
            CreatedAt = DateTime.UtcNow
        };

        _context.Roadmaps.Add(roadmap);
        await contextSaveAsync();

        // Generate milestones across Beginner, Intermediate, Advanced
        var milestones = GenerateMilestonesForCareer(roadmap.Id, career);
        _context.RoadmapMilestones.AddRange(milestones);
        await _context.SaveChangesAsync();

        return await _context.Roadmaps
            .Include(r => r.Milestones.OrderBy(m => m.OrderNumber))
            .Include(r => r.Career)
            .FirstAsync(r => r.Id == roadmap.Id);
    }

    private async Task contextSaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Roadmap>> GetUserRoadmapsAsync(string userId)
    {
        return await _context.Roadmaps
            .Include(r => r.Career)
            .Include(r => r.Milestones)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Roadmap?> GetRoadmapByIdAsync(int roadmapId, string userId)
    {
        return await _context.Roadmaps
            .Include(r => r.Career)
                .ThenInclude(c => c!.CareerSkills)
                    .ThenInclude(cs => cs.Skill)
            .Include(r => r.Milestones.OrderBy(m => m.OrderNumber))
            .FirstOrDefaultAsync(r => r.Id == roadmapId && r.UserId == userId);
    }

    public async Task<bool> ToggleMilestoneCompletionAsync(int milestoneId, string userId)
    {
        var milestone = await _context.RoadmapMilestones
            .Include(m => m.Roadmap)
            .FirstOrDefaultAsync(m => m.Id == milestoneId && m.Roadmap != null && m.Roadmap.UserId == userId);

        if (milestone == null) return false;

        milestone.IsCompleted = !milestone.IsCompleted;
        milestone.CompletedAt = milestone.IsCompleted ? DateTime.UtcNow : null;
        if (milestone.Roadmap != null)
        {
            milestone.Roadmap.LastUpdated = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<double> CalculateRoadmapProgressAsync(int roadmapId)
    {
        var milestones = await _context.RoadmapMilestones
            .Where(m => m.RoadmapId == roadmapId)
            .ToListAsync();

        if (!milestones.Any()) return 0.0;

        int completed = milestones.Count(m => m.IsCompleted);
        return Math.Round((double)completed / milestones.Count * 100.0, 1);
    }

    private static List<RoadmapMilestone> GenerateMilestonesForCareer(int roadmapId, Career career)
    {
        var list = new List<RoadmapMilestone>();
        var title = career.Title;

        // Stage 1: Beginner (Foundations)
        list.Add(new RoadmapMilestone
        {
            RoadmapId = roadmapId,
            Stage = "Beginner",
            Title = "Phase 1: Core Fundamentals & Principles",
            Description = $"Master foundational syntax, fundamental computer science concepts, memory models, and version control essential for {title}.",
            RecommendedSkills = GetSkillsForStage(career, 1),
            SuggestedProjects = $"Build a console-based command-line application or basic utility adhering to clean code standards and commit with Git.",
            OrderNumber = 1
        });

        list.Add(new RoadmapMilestone
        {
            RoadmapId = roadmapId,
            Stage = "Beginner",
            Title = "Phase 2: Relational Data & Persistence Basics",
            Description = "Learn relational database modeling, structured SQL querying (SELECT, JOIN, GROUP BY), normalization, and data validation.",
            RecommendedSkills = "SQL, Relational Modeling, Git, Data Integrity",
            SuggestedProjects = "Design a normalized database schema with SQLite or PostgreSQL; write transactional queries and indices.",
            OrderNumber = 2
        });

        // Stage 2: Intermediate (Application Building)
        list.Add(new RoadmapMilestone
        {
            RoadmapId = roadmapId,
            Stage = "Intermediate",
            Title = "Phase 3: Web Services, Frameworks & Architecture",
            Description = $"Implement production-ready modular application components, RESTful API endpoints, and error handling for {title}.",
            RecommendedSkills = GetSkillsForStage(career, 2),
            SuggestedProjects = $"Develop a fully functional RESTful service or interactive dashboard with authentication, CRUD operations, and pagination.",
            OrderNumber = 3
        });

        list.Add(new RoadmapMilestone
        {
            RoadmapId = roadmapId,
            Stage = "Intermediate",
            Title = "Phase 4: Automated Testing & Continuous Integration",
            Description = "Write unit tests, mock external dependencies, perform integration verification, and setup GitHub Actions CI workflows.",
            RecommendedSkills = "Automated Testing, CI/CD Pipelines, Code Coverage, Refactoring",
            SuggestedProjects = "Achieve 80%+ unit test coverage on core business logic; configure automated pull-request validation builds.",
            OrderNumber = 4
        });

        // Stage 3: Advanced (Production & Scale)
        list.Add(new RoadmapMilestone
        {
            RoadmapId = roadmapId,
            Stage = "Advanced",
            Title = "Phase 5: Cloud Deployment & Containerization",
            Description = "Containerize the application with Docker, configure environment secrets, configure reverse proxies, and deploy to cloud infrastructure.",
            RecommendedSkills = "Docker, Cloud Platforms (AWS/Azure), Linux, Security Best Practices",
            SuggestedProjects = $"Containerize your {title} application with a multi-stage Dockerfile and deploy to cloud instances with HTTPS.",
            OrderNumber = 5
        });

        list.Add(new RoadmapMilestone
        {
            RoadmapId = roadmapId,
            Stage = "Advanced",
            Title = "Phase 6: Performance Optimization & Portfolio Capstone",
            Description = $"Profile bottlenecks, add caching layers, audit security compliance, and finalize an executive portfolio demonstration.",
            RecommendedSkills = "Performance Tuning, High Availability, System Design, Communication",
            SuggestedProjects = $"Complete an end-to-end production capstone demonstrating scalable architecture, database optimization, and documentation.",
            OrderNumber = 6
        });

        return list;
    }

    private static string GetSkillsForStage(Career career, int stage)
    {
        var skills = career.CareerSkills.Select(cs => cs.Skill?.Name).Where(n => !string.IsNullOrEmpty(n)).ToList();
        if (!skills.Any()) return "Core domain tools, Git, Relational Database";

        if (stage == 1)
        {
            return string.Join(", ", skills.Take(Math.Min(3, skills.Count)));
        }
        else if (stage == 2)
        {
            return string.Join(", ", skills.Skip(1).Take(Math.Min(4, skills.Count)));
        }
        else
        {
            return string.Join(", ", skills.TakeLast(Math.Min(3, skills.Count)));
        }
    }
}
