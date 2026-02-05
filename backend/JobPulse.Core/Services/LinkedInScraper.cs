using JobPulse.Core.Models;
using JobPulse.Core.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace JobPulse.Core.Services
{
    public class LinkedInScraper : IJobScraper
    {
        private const string BaseUrl = "https://www.linkedin.com";
        private const int JobsPerPage = 25;

        private readonly HttpClient _httpClient;

        public LinkedInScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public string Source => throw new NotImplementedException();

        public Task<List<Job>> ScrapeAsync(SearchRequest request)
        {
            var allJobs = new List<Job>();
            var start = 0;

            var scrul = BuildSearchUrl(new SearchRequest(), start);
            throw new NotImplementedException();
        }


        /// <summary>
        /// Builds the LinkedIn guest search URL with query parameters.
        /// Example: https://www.linkedin.com/jobs-guest/jobs/api/seeMoreJobPostings/search?keywords=developer&location=Vancouver&start=0
        /// </summary>
        private static string BuildSearchUrl(SearchRequest request, int start)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrEmpty(request.Keywords))
                query["keywords"] = request.Keywords;

            if (!string.IsNullOrEmpty(request.Location))
                query["location"] = request.Location;

            if (request.IsRemote == true)
                query["f_WT"] = "2"; // LinkedIn remote filter code

            if (request.HoursOld.HasValue)
                query["f_TPR"] = $"r{request.HoursOld.Value * 3600}"; // Time posted filter in seconds

            query["start"] = start.ToString();

            return $"{BaseUrl}/jobs-guest/jobs/api/seeMoreJobPostings/search?{query}";
        }

    }
}
