using CyberAware.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace CyberAware.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<ContentSection> ContentSections { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Enrolment> Enrolments { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Attempt> Attempts { get; set; }
    public DbSet<Answer> Answers { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<ModuleVersion> ModuleVersions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ROLES
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(r => r.RoleID);
            entity.Property(r => r.RoleName).IsRequired().HasMaxLength(50);
            entity.HasIndex(r => r.RoleName).IsUnique();
        });

        // USERS
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.UserID);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            entity.Property(u => u.TwoFactorSecret).HasMaxLength(255);
            entity.Property(u => u.IsTwoFactorEnabled).HasDefaultValue(false);
            entity.Property(u => u.IsActive).HasDefaultValue(true);
            entity.Property(u => u.CreatedAt).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(u => u.Role)
                  .WithMany(r => r.Users)
                  .HasForeignKey(u => u.RoleID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // MODULES
        modelBuilder.Entity<Module>(entity =>
        {
            entity.HasKey(m => m.ModuleID);
            entity.Property(m => m.Title).IsRequired().HasMaxLength(200);
            entity.Property(m => m.IsPublished).HasDefaultValue(false);
            entity.Property(m => m.PassMark).HasDefaultValue(50);
            entity.Property(m => m.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(m => m.UpdatedAt).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(m => m.Creator)
                  .WithMany(u => u.CreatedModules)
                  .HasForeignKey(m => m.CreatedBy)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // CONTENT SECTIONS
        modelBuilder.Entity<ContentSection>(entity =>
        {
            entity.HasKey(c => c.SectionID);
            entity.Property(c => c.Title).IsRequired().HasMaxLength(200);
            entity.Property(c => c.ContentBody).IsRequired();

            entity.HasOne(c => c.Module)
                  .WithMany(m => m.ContentSections)
                  .HasForeignKey(c => c.ModuleID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // QUESTIONS
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(q => q.QuestionID);
            entity.Property(q => q.QuestionText).IsRequired();
            entity.Property(q => q.DifficultyLevel).IsRequired().HasMaxLength(20);

            entity.HasOne(q => q.Module)
                  .WithMany(m => m.Questions)
                  .HasForeignKey(q => q.ModuleID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // OPTIONS
        modelBuilder.Entity<Option>(entity =>
        {
            entity.HasKey(o => o.OptionID);
            entity.Property(o => o.OptionText).IsRequired().HasMaxLength(500);
            entity.Property(o => o.IsCorrect).HasDefaultValue(false);

            entity.HasOne(o => o.Question)
                  .WithMany(q => q.Options)
                  .HasForeignKey(o => o.QuestionID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ENROLMENTS
        modelBuilder.Entity<Enrolment>(entity =>
        {
            entity.HasKey(e => e.EnrolmentID);
            entity.Property(e => e.PathwayName).HasMaxLength(150);
            entity.Property(e => e.EnrolledAt).HasDefaultValueSql("SYSDATETIME()");
            entity.HasIndex(e => new { e.UserID, e.ModuleID }).IsUnique();

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Enrolments)
                  .HasForeignKey(e => e.UserID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Module)
                  .WithMany(m => m.Enrolments)
                  .HasForeignKey(e => e.ModuleID)
                  .OnDelete(DeleteBehavior.NoAction);

            // IMPORTANT: No inverse navigation - avoids the two-FK-to-User conflict
            entity.HasOne(e => e.Enroller)
                  .WithMany()
                  .HasForeignKey(e => e.EnrolledBy)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // SCHEDULES
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(s => s.ScheduleID);
            entity.Property(s => s.FrequencyDays).IsRequired();
            entity.Property(s => s.NextDueDate).IsRequired();

            entity.HasOne(s => s.Module)
                  .WithMany(m => m.Schedules)
                  .HasForeignKey(s => s.ModuleID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(s => s.Enrolment)
                  .WithMany(e => e.Schedules)
                  .HasForeignKey(s => s.EnrolmentID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ATTEMPTS
        modelBuilder.Entity<Attempt>(entity =>
        {
            entity.HasKey(a => a.AttemptID);
            entity.Property(a => a.Score).IsRequired();
            entity.Property(a => a.Passed).IsRequired();
            entity.Property(a => a.AttemptDate).HasDefaultValueSql("SYSDATETIME()");
            entity.Property(a => a.DurationSeconds).IsRequired();

            entity.HasOne(a => a.User)
                  .WithMany(u => u.Attempts)
                  .HasForeignKey(a => a.UserID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(a => a.Module)
                  .WithMany(m => m.Attempts)
                  .HasForeignKey(a => a.ModuleID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // ANSWERS
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(a => a.AnswerID);
            entity.HasIndex(a => new { a.AttemptID, a.QuestionID }).IsUnique();

            entity.HasOne(a => a.Attempt)
                  .WithMany(at => at.Answers)
                  .HasForeignKey(a => a.AttemptID)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.Question)
                  .WithMany(q => q.Answers)
                  .HasForeignKey(a => a.QuestionID)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(a => a.SelectedOption)
                  .WithMany()
                  .HasForeignKey(a => a.SelectedOptionID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // AUDIT LOGS
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(a => a.LogID);
            entity.Property(a => a.Action).IsRequired().HasMaxLength(100);
            entity.Property(a => a.TargetTable).HasMaxLength(100);
            entity.Property(a => a.Timestamp).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(a => a.User)
                  .WithMany(u => u.AuditLogs)
                  .HasForeignKey(a => a.UserID)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // NOTIFICATIONS
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(n => n.NotificationID);
            entity.Property(n => n.Message).IsRequired().HasMaxLength(500);
            entity.Property(n => n.IsRead).HasDefaultValue(false);
            entity.Property(n => n.CreatedAt).HasDefaultValueSql("SYSDATETIME()");

            entity.HasOne(n => n.User)
                  .WithMany(u => u.Notifications)
                  .HasForeignKey(n => n.UserID)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // MODULE VERSIONS
        modelBuilder.Entity<ModuleVersion>(entity =>
        {
            entity.HasKey(mv => mv.VersionID);
            entity.Property(mv => mv.ContentSnapshot).IsRequired();
            entity.Property(mv => mv.CreatedAt).HasDefaultValueSql("SYSDATETIME()");
            entity.HasIndex(mv => new { mv.ModuleID, mv.VersionNumber }).IsUnique();

            entity.HasOne(mv => mv.Module)
                  .WithMany(m => m.ModuleVersions)
                  .HasForeignKey(mv => mv.ModuleID)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(mv => mv.Creator)
                  .WithMany()
                  .HasForeignKey(mv => mv.CreatedBy)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }
}