using CareerFit.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerFit.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();
    public DbSet<CareerCategory> CareerCategories => Set<CareerCategory>();
    public DbSet<Career> Careers => Set<Career>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<CareerSkill> CareerSkills => Set<CareerSkill>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<Assessment> Assessments => Set<Assessment>();
    public DbSet<AssessmentAnswer> AssessmentAnswers => Set<AssessmentAnswer>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();
    public DbSet<SavedCareer> SavedCareers => Set<SavedCareer>();
    public DbSet<Roadmap> Roadmaps => Set<Roadmap>();
    public DbSet<RoadmapMilestone> RoadmapMilestones => Set<RoadmapMilestone>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // CareerSkill composite key
        builder.Entity<CareerSkill>()
            .HasKey(cs => new { cs.CareerId, cs.SkillId });

        builder.Entity<CareerSkill>()
            .HasOne(cs => cs.Career)
            .WithMany(c => c.CareerSkills)
            .HasForeignKey(cs => cs.CareerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CareerSkill>()
            .HasOne(cs => cs.Skill)
            .WithMany(s => s.CareerSkills)
            .HasForeignKey(cs => cs.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        // SavedCareer unique bookmark per user/career
        builder.Entity<SavedCareer>()
            .HasIndex(sc => new { sc.UserId, sc.CareerId })
            .IsUnique();

        builder.Entity<SavedCareer>()
            .HasOne(sc => sc.Career)
            .WithMany(c => c.SavedCareers)
            .HasForeignKey(sc => sc.CareerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<SavedCareer>()
            .HasOne(sc => sc.User)
            .WithMany(u => u.SavedCareers)
            .HasForeignKey(sc => sc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // CandidateProfile 1-to-1 with ApplicationUser
        builder.Entity<CandidateProfile>()
            .HasIndex(cp => cp.UserId)
            .IsUnique();

        builder.Entity<CandidateProfile>()
            .HasOne(cp => cp.User)
            .WithOne(u => u.CandidateProfile)
            .HasForeignKey<CandidateProfile>(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserSkills
        builder.Entity<UserSkill>()
            .HasOne(us => us.CandidateProfile)
            .WithMany(cp => cp.UserSkills)
            .HasForeignKey(us => us.CandidateProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserSkill>()
            .HasOne(us => us.Skill)
            .WithMany(s => s.UserSkills)
            .HasForeignKey(us => us.SkillId)
            .OnDelete(DeleteBehavior.Cascade);

        // Recommendations
        builder.Entity<Recommendation>()
            .HasOne(r => r.Career)
            .WithMany(c => c.Recommendations)
            .HasForeignKey(r => r.CareerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Recommendation>()
            .HasOne(r => r.Assessment)
            .WithMany(a => a.Recommendations)
            .HasForeignKey(r => r.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Roadmaps
        builder.Entity<Roadmap>()
            .HasOne(rm => rm.Career)
            .WithMany(c => c.Roadmaps)
            .HasForeignKey(rm => rm.CareerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Roadmap>()
            .HasMany(rm => rm.Milestones)
            .WithOne(m => m.Roadmap)
            .HasForeignKey(m => m.RoadmapId)
            .OnDelete(DeleteBehavior.Cascade);

        // Questions & Options
        builder.Entity<Question>()
            .HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AssessmentAnswer>()
            .HasOne(aa => aa.Assessment)
            .WithMany(a => a.Answers)
            .HasForeignKey(aa => aa.AssessmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<AssessmentAnswer>()
            .HasOne(aa => aa.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(aa => aa.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.Entity<Career>().HasIndex(c => c.Category);
        builder.Entity<Career>().HasIndex(c => c.ExperienceLevel);
        builder.Entity<Skill>().HasIndex(s => s.Name);
        builder.Entity<Question>().HasIndex(q => q.SectionKey);
    }
}
