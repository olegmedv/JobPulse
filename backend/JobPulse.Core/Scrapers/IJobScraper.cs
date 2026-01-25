using JobPulse.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPulse.Core.Scrapers
{
    public interface IJobScraper
    {
        /// <summary>
        /// Source name, e.g. "LinkedIn"
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Searches for jobs based on given parameters
        /// </summary>
        /// <param name="request">Search parameters</param>
        /// <returns>List of found jobs</returns>
        Task<List<Job>> ScrapeAsync(SearchRequest request);
    }
}
