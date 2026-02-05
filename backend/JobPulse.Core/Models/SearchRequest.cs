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
        /// Filter by posting age in minutes (60=1hour, 30=30min)
        /// </summary>
        public int MinutesOld { get; set; } = 30;

        /// <summary>
        /// Filter for remote jobs only
        /// </summary>
        public bool? IsRemote { get; set; }

        /// <summary>
        /// Filter for Easy Apply jobs only
        /// </summary>
        public bool? EasyApply { get; set; }
    }
}
