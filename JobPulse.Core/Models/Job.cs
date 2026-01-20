using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPulse.Core.Models
{
    /// <summary>
    /// Job model - unified format for all sources
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Unique job identifier (from source)
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Position title, e.g. "C# Developer"
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Company name, e.g. "Microsoft"
        /// </summary>
        public string Company { get; set; } = string.Empty;

        /// <summary>
        /// Location, e.g. "Vancouver, BC, Canada"
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// Direct link to the job posting
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Source: "LinkedIn", "Indeed", "Glassdoor"
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// Date posted (can be null if unknown)
        /// </summary>
        public DateTime? DatePosted { get; set; }

        /// <summary>
        /// Job description (optional, for detailed view)
        /// </summary>
        public string? Description { get; set; }
    }
}
