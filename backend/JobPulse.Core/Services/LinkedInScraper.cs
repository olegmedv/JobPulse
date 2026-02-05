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

            var jobList = ParseJobs(html, request.IsRemote);

            return jobList;
        }


        /// <summary>
        /// Parses the HTML response into a list of Job objects.
        /// </summary>
        private List<Job> ParseJobs(string html, bool? isRemote)
        {
            var jobs = new List<Job>();
            var seenIds = new HashSet<string>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Each job listing is a div with class "base-search-card"
            var jobCards = doc.DocumentNode
                .SelectNodes("//div[contains(@class, 'base-search-card')]");

            if (jobCards == null || jobCards.Count == 0)
                return jobs;

            foreach (var card in jobCards)
            {
                try
                {
                    var job = ParseJobCard(card, seenIds, isRemote);
                    if (job != null)
                        jobs.Add(job);
                }
                catch
                {
                    // Skip malformed cards
                }
            }

            return jobs;
        }

        /// <summary>
        /// Parses one job card from HTML into a Job object.
        /// XPath selectors from JobSpyNet reference project.
        /// </summary>
        private Job? ParseJobCard(HtmlNode card, HashSet<string> seenIds, bool? isRemote)
        {
            // 1. Job link — contains the job ID in URL
            var linkNode = card.SelectSingleNode(
                ".//a[contains(@class, 'base-card__full-link')]");
            if (linkNode == null) return null;

            var href = linkNode.GetAttributeValue("href", "");
            var jobId = ExtractJobId(href);

            if (string.IsNullOrEmpty(jobId) || seenIds.Contains(jobId))
                return null;

            seenIds.Add(jobId);

            // 2. Title
            var titleNode = card.SelectSingleNode(
                ".//span[contains(@class, 'sr-only')]");
            var title = titleNode?.InnerText.Trim() ?? "N/A";

            // 3. Company
            var companyNode = card.SelectSingleNode(
                ".//h4[contains(@class, 'base-search-card__subtitle')]//a");
            var company = companyNode?.InnerText.Trim() ?? "N/A";

            // 4. Location
            var locationNode = card.SelectSingleNode(
                ".//span[contains(@class, 'job-search-card__location')]");
            var location = locationNode?.InnerText.Trim() ?? "";

            // 5. Date posted
            DateTime? datePosted = null;
            var dateNode = card.SelectSingleNode(
                ".//time[contains(@class, 'job-search-card__listdate')]");
            if (dateNode != null)
            {
                var dateStr = dateNode.GetAttributeValue("datetime", "");
                if (DateTime.TryParse(dateStr, out var parsed))
                    datePosted = parsed;
            }

            // 6. Salary (optional)
            decimal? salaryMin = null;
            decimal? salaryMax = null;
            var salaryNode = card.SelectSingleNode(
                ".//span[contains(@class, 'job-search-card__salary-info')]");
            if (salaryNode != null)
            {
                (salaryMin, salaryMax) = ParseSalary(salaryNode.InnerText.Trim());
            }

            return new Job
            {
                Id = $"li-{jobId}",
                Title = HttpUtility.HtmlDecode(title),
                Company = HttpUtility.HtmlDecode(company),
                Location = location,
                Url = $"{BaseUrl}/jobs/view/{jobId}",
                Source = Source,
                DatePosted = datePosted,
                IsRemote = isRemote,
                EasyApply = null,
                SalaryMin = salaryMin,
                SalaryMax = salaryMax
            };
        }

        /// <summary>
        /// Extracts job ID from LinkedIn URL.
        /// Input:  ".../software-engineer-at-google-3812345678?refId=..."
        /// Output: "3812345678"
        /// </summary>
        private static string? ExtractJobId(string href)
        {
            if (string.IsNullOrEmpty(href))
            {
                return null;
            }

            // Remove query string: "...?refId=xxx" -> "..."
            var cleanUrl = href.Split('?')[0];

            // Split by dash to get the last segment (job ID)
            var parts = cleanUrl.Split('-');

            if (parts.Length == 0)
            {
                return null;
            }

            var jobId = parts[parts.Length - 1];
            return jobId;
        }

        /// <summary>
        /// Parses "$80,000 - $120,000/yr" into (80000, 120000).
        /// </summary>
        private static (decimal? min, decimal? max) ParseSalary(string salaryText)
        {
            if (string.IsNullOrWhiteSpace(salaryText))
                return (null, null);

            var parts = salaryText.Split('-', '–', '—')
                .Select(p => p.Trim()).ToArray();

            if (parts.Length < 2)
                return (null, null);

            return (ParseCurrency(parts[0]), ParseCurrency(parts[1]));
        }

        /// <summary>
        /// "$80,000/yr" → 80000
        /// </summary>
        private static decimal? ParseCurrency(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var cleaned = new string(text
                .Where(c => char.IsDigit(c) || c == '.').ToArray());

            return decimal.TryParse(cleaned, out var amount) ? amount : null;
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
                query["f_WT"] = "2";

            query["f_TPR"] = $"r{request.MinutesOld * 60}";

            return $"{BaseUrl}/jobs-guest/jobs/api/seeMoreJobPostings/search?{query}";
        }

    }
}
