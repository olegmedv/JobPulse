using JobPulse.Core.Data;
using JobPulse.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JobPulse.Core.Services;

/// <summary>
/// Background service for scheduled job searches.
/// Runs periodically via Hangfire to fetch new jobs for all users with saved preferences.
/// </summary>
public class JobSearchBackgroundService
{
    private readonly JobPulseDbContext _db;
    private readonly JobService _jobService;
    private readonly ILogger<JobSearchBackgroundService> _logger;

    public JobSearchBackgroundService(
        JobPulseDbContext db,
        JobService jobService,
        ILogger<JobSearchBackgroundService> logger)
    {
        _db = db;
        _jobService = jobService;
        _logger = logger;
    }

    /// <summary>
    /// Runs job search for all users with saved preferences.
    /// Called by Hangfire on schedule.
    /// </summary>
    public async Task SearchForAllUsersAsync()
    {
        _logger.LogInformation("Starting scheduled job search for all users");

        var preferences = await _db.UserPreferences
            .Where(p => p.NotificationsEnabled)
            .ToListAsync();

        _logger.LogInformation("Found {Count} users with notifications enabled", preferences.Count);

        foreach (var pref in preferences)
        {
            try
            {
                await SearchForUserAsync(pref);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jobs for user {UserId}", pref.UserId);
            }
        }

        _logger.LogInformation("Completed scheduled job search");
    }

    /// <summary>
    /// Runs job search for a single user based on their preferences.
    /// Saves new jobs to database and marks them for notification.
    /// </summary>
    private async Task SearchForUserAsync(UserPreferences pref)
    {
        _logger.LogInformation("Searching jobs for user {UserId}: {Keywords} in {Location}",
            pref.UserId, pref.Keywords, pref.Location);

        var request = new SearchRequest
        {
            Keywords = pref.Keywords,
            Location = pref.Location,
            IsRemote = pref.IsRemote
        };

        var jobs = await _jobService.SearchAsync(request);

        if (jobs.Count == 0)
        {
            _logger.LogInformation("No jobs found for user {UserId}", pref.UserId);
            return;
        }

        // Get existing job URLs to avoid duplicates
        var existingUrls = await _db.Jobs
            .Where(j => j.Source == "LinkedIn")
            .Select(j => j.Url)
            .ToHashSetAsync();

        var newJobs = jobs
            .Where(j => !string.IsNullOrEmpty(j.Url) && !existingUrls.Contains(j.Url))
            .ToList();

        if (newJobs.Count == 0)
        {
            _logger.LogInformation("No new jobs for user {UserId}", pref.UserId);
            return;
        }

        // Save new jobs
        _db.Jobs.AddRange(newJobs);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Saved {Count} new jobs for user {UserId}", newJobs.Count, pref.UserId);

        // Mark jobs for notification (will be sent by notification service later)
        var notifications = newJobs.Select(job => new UserNotifiedJob
        {
            UserId = pref.UserId,
            JobId = job.Id,
            NotifiedAt = DateTime.UtcNow
        });

        _db.UserNotifiedJobs.AddRange(notifications);
        await _db.SaveChangesAsync();
    }
}
