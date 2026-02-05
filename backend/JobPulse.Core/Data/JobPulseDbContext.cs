using JobPulse.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace JobPulse.Core.Data;

public class JobPulseDbContext : DbContext
{
    public JobPulseDbContext(DbContextOptions<JobPulseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Job> Jobs { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }
    public DbSet<UserNotifiedJob> UserNotifiedJobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Job>(entity =>
        {
            entity.ToTable("jobs");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title").IsRequired();
            entity.Property(e => e.Company).HasColumnName("company");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Url).HasColumnName("url").IsRequired();
            entity.Property(e => e.Source).HasColumnName("source");
            entity.Property(e => e.DatePosted).HasColumnName("date_posted");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsRemote).HasColumnName("is_remote");
            entity.Property(e => e.SalaryMin).HasColumnName("salary_min");
            entity.Property(e => e.SalaryMax).HasColumnName("salary_max");
            entity.Property(e => e.EasyApply).HasColumnName("easy_apply");
            entity.Property(e => e.JobType).HasColumnName("job_type");
            entity.Property(e => e.JobLevel).HasColumnName("job_level");
            entity.Property(e => e.Industry).HasColumnName("industry");
            entity.Property(e => e.JobFunction).HasColumnName("job_function");
            entity.Property(e => e.DirectApplyUrl).HasColumnName("direct_apply_url");
        });

        modelBuilder.Entity<UserPreferences>(entity =>
        {
            entity.ToTable("user_preferences");
            entity.HasKey(e => e.UserId);
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email).HasColumnName("email").IsRequired();
            entity.Property(e => e.Keywords).HasColumnName("keywords").IsRequired();
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.IsRemote).HasColumnName("is_remote");
            entity.Property(e => e.FilterKeywords).HasColumnName("filter_keywords");
            entity.Property(e => e.NotificationsEnabled).HasColumnName("notifications_enabled");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
        });

        modelBuilder.Entity<UserNotifiedJob>(entity =>
        {
            entity.ToTable("user_notified_jobs");
            entity.HasKey(e => new { e.UserId, e.JobId });
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.NotifiedAt).HasColumnName("notified_at");
            entity.HasOne(e => e.Job)
                  .WithMany()
                  .HasForeignKey(e => e.JobId);
        });
    }
}
