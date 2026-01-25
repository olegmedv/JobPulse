using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPulse.Core.Models
{
    /// <summary>
    /// Search parameters from user
    /// </summary>
    public class SearchRequest
    {
        /// <summary>
        /// Keywords for search, e.g. "C# developer"
        /// </summary>
        public string Keywords { get; set; } = string.Empty;

        /// <summary>
        /// Location for search, e.g. "Canada" or "Vancouver"
        /// </summary>
        public string Location { get; set; } = string.Empty;

        /// <summary>
        /// How many results to return (default 25)
        /// </summary>
        public int ResultsWanted { get; set; } = 25;

        /// <summary>
        /// Filter by posting age in hours (24=day, 168=week)
        /// </summary>
        public int? HoursOld { get; set; }

        /// <summary>
        /// Filter for remote jobs only
        /// </summary>
        public bool? IsRemote { get; set; }
    }
}
