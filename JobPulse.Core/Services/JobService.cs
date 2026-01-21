using JobPulse.Core.Models;
using JobPulse.Core.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPulse.Core.Services
{
    /// <summary>
    /// Service for searching jobs.
    /// Manages scrapers and combines results.
    /// </summary>
    public class JobService
    {
        private readonly IEnumerable<IJobScraper> _scrapers;

        /// <summary>
        /// Constructor. Receives all registered scrapers via DI.
        /// </summary>
        /// <param name="scrapers">Collection of scrapers (LinkedIn, Indeed, etc.)</param>
        public JobService(IEnumerable<IJobScraper> scrapers)
        {
            _scrapers = scrapers;
        }

        /// <summary>
        /// Searches for jobs across all sources
        /// </summary>
        /// <param name="request">Search parameters</param>
        /// <returns>Combined list of jobs</returns>
        public async Task<List<Job>> SearchAsync(SearchRequest request)
        {
            return null;
        }
    }
}
