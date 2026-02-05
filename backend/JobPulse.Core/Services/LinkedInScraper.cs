using HtmlAgilityPack;
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
        public string Source => "LinkedIn";

        private readonly HttpClient _httpClient;

        public LinkedInScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Fetches recent jobs from LinkedIn. One request, no pagination.
        /// For 30-min interval, set HoursOld=1 in SearchRequest (1 hour window with overlap).
        /// Returns up to 25 jobs (one page).
        /// </summary>
        public async Task<List<Job>> ScrapeAsync(SearchRequest request)
        {
            var url = BuildSearchUrl(request);
            ApplyHeaders();

            var response = await _httpClient.GetAsync(url);

            // 429 = rate limited by LinkedIn
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                return new List<Job>();

            if (!response.IsSuccessStatusCode)
                return new List<Job>();

            var html = await response.Content.ReadAsStringAsync();

            // Very short response = blocked or empty
            if (html.Length < 100)
                return new List<Job>();

            var jobList = ParseJobs(html);

            return jobList;
        }


        /// <summary>
        /// Parses the HTML response into a list of Job objects.
        /// </summary>
        private List<Job> ParseJobs(string html)
        {
            var jobs = new List<Job>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return jobs;
        }

        /// <summary>
        /// Sets Chrome-like headers so LinkedIn doesn't block us.
        /// Do NOT set Accept-Encoding — AutomaticDecompression in Program.cs handles that.
        /// </summary>
        private void ApplyHeaders()
        {
            _httpClient.DefaultRequestHeaders.Clear();

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language",
                "en-US,en;q=0.5");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control",
                "max-age=0");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Upgrade-Insecure-Requests",
                "1");
        }

        /// <summary>
        /// Builds the LinkedIn guest search URL with query parameters.
        /// Example: https://www.linkedin.com/jobs-guest/jobs/api/seeMoreJobPostings/search?keywords=developer&location=Vancouver&start=0
        /// </summary>
        private static string BuildSearchUrl(SearchRequest request)
        {
            var query = HttpUtility.ParseQueryString(string.Empty);

            if (!string.IsNullOrEmpty(request.Keywords))
                query["keywords"] = request.Keywords;

            if (!string.IsNullOrEmpty(request.Location))
                query["location"] = request.Location;

            if (request.IsRemote == true)
                query["f_WT"] = "2"; // LinkedIn remote filter

            if (request.EasyApply == true)
    query["f_AL"] = "true"; // LinkedIn Easy Apply filter

            query["f_TPR"] = $"r{request.MinutesOld * 60}"; // Convert minutes to seconds

            return $"{BaseUrl}/jobs-guest/jobs/api/seeMoreJobPostings/search?{query}";
        }

    }
}
