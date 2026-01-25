using System;

namespace JobPulse.Core.Models
{
    /// <summary>
    /// User search preferences and notification settings
    /// </summary>
    public class UserPreferences
    {
        /// <summary>
        /// User ID from Supabase Auth (UUID)
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Email for notifications
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Search keywords for LinkedIn, e.g. "developer"
        /// </summary>
        public string Keywords { get; set; } = string.Empty;

        /// <summary>
        /// Location for search, e.g. "Vancouver"
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Filter for remote jobs only
        /// </summary>
        public bool? IsRemote { get; set; }

        /// <summary>
        /// Post-filter keywords, comma-separated, e.g. "C#, .NET, dotnet"
        /// Job must contain at least one to match
        /// </summary>
        public string? FilterKeywords { get; set; }

        /// <summary>
        /// Whether to send email notifications
        /// </summary>
        public bool NotificationsEnabled { get; set; } = true;

        /// <summary>
        /// When preferences were created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
