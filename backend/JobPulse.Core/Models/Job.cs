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

        /// <summary>
        /// Is this a remote position?
        /// </summary>
        public bool? IsRemote { get; set; }

        /// <summary>
        /// Is this an Easy Apply job? (apply via LinkedIn)
        /// </summary>
        public bool? EasyApply { get; set; }

        /// <summary>
        /// Minimum salary (if available)
        /// </summary>
        public decimal? SalaryMin { get; set; }

        /// <summary>
        /// Maximum salary (if available)
        /// </summary>
        public decimal? SalaryMax { get; set; }

        /// <summary>
        /// Employment type: Full-time, Part-time, Contract, etc.
        /// </summary>
        public string? JobType { get; set; }

        /// <summary>
        /// Seniority level: Entry, Mid-Senior, Director, etc.
        /// </summary>
        public string? JobLevel { get; set; }

        /// <summary>
        /// Company industry: IT Services, Healthcare, etc.
        /// </summary>
        public string? Industry { get; set; }

        /// <summary>
        /// Job function: Engineering, Sales, Marketing, etc.
        /// </summary>
        public string? JobFunction { get; set; }

        /// <summary>
        /// External apply URL (if not Easy Apply)
        /// </summary>
        public string? DirectApplyUrl { get; set; }
    }
}
