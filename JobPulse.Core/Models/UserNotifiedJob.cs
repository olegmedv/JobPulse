using System;

namespace JobPulse.Core.Models
{
    /// <summary>
    /// Tracks which jobs were sent to which users (to avoid duplicates)
    /// </summary>
    public class UserNotifiedJob
    {
        /// <summary>
        /// User ID from Supabase Auth (UUID)
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Job ID (FK to Jobs table)
        /// </summary>
        public string JobId { get; set; } = string.Empty;

        /// <summary>
        /// When the notification was sent
        /// </summary>
        public DateTime NotifiedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Navigation property to Job
        /// </summary>
        public Job? Job { get; set; }
    }
}
