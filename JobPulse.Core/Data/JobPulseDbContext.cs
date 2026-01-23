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
        });
    }
}
