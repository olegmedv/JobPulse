using HtmlAgilityPack;
using JobPulse.Core.Models;
using JobPulse.Core.Scrapers;
using System.Text.RegularExpressions;
using System.Web;

namespace JobPulse.Core.Services
{
    public class LinkedInScraper : IJobScraper
    {
        private const string BaseUrl = "https://www.linkedin.com";
        private static readonly string[] RemoteKeywords = { "remote", "work from home", "wfh" };

        public string Source => "LinkedIn";

        private readonly HttpClient _httpClient;

        public LinkedInScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Scrapes LinkedIn job listings based on the search request.
        /// Fetches search results, then fetches details for each job.
        /// </summary>
        public async Task<List<Job>> ScrapeAsync(SearchRequest request)
        {
            var url = BuildSearchUrl(request);
            ApplyHeaders();

            var response = await _httpClient.GetAsync(url);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                return new List<Job>();

            if (!response.IsSuccessStatusCode)
                return new List<Job>();

            var html = await response.Content.ReadAsStringAsync();

            if (html.Length < 100)
                return new List<Job>();

            var jobs = ParseJobs(html);

            // Fetch details for each job
            foreach (var job in jobs)
            {
                await FetchJobDetails(job, request.IsRemote);
                await Task.Delay(500);
            }

            return jobs;
        }

        /// <summary>
        /// Parses job cards from the search results HTML.
        /// Each card contains: title, company, location, date, salary (optional).
        /// </summary>
        private List<Job> ParseJobs(string html)
        {
            var jobs = new List<Job>();
            var seenIds = new HashSet<string>();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var jobCards = doc.DocumentNode
                .SelectNodes("//div[contains(@class, 'base-search-card')]");

            if (jobCards == null || jobCards.Count == 0)
                return jobs;

            foreach (var card in jobCards)
            {
                try
                {
                    var job = ParseJobCard(card, seenIds);
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
        /// Parses a single job card.
        /// </summary>
        private Job? ParseJobCard(HtmlNode card, HashSet<string> seenIds)
        {
            var linkNode = card.SelectSingleNode(
                ".//a[contains(@class, 'base-card__full-link')]");
            if (linkNode == null) return null;

            var href = linkNode.GetAttributeValue("href", "");
            var jobId = ExtractJobId(href);

            if (string.IsNullOrEmpty(jobId) || seenIds.Contains(jobId))
                return null;

            seenIds.Add(jobId);

            // 1. Title
            var titleNode = card.SelectSingleNode(".//span[contains(@class, 'sr-only')]");
            var title = titleNode?.InnerText.Trim() ?? "N/A";

            // 2. Company
            var companyNode = card.SelectSingleNode(
                ".//h4[contains(@class, 'base-search-card__subtitle')]//a");
            var company = companyNode?.InnerText.Trim() ?? "N/A";

            // 3. Location
            var locationNode = card.SelectSingleNode(
                ".//span[contains(@class, 'job-search-card__location')]");
            var location = locationNode?.InnerText.Trim() ?? "";

            // 4. Date posted
            DateTime? datePosted = null;
            var dateNode = card.SelectSingleNode(
                ".//time[contains(@class, 'job-search-card__listdate')]");
            if (dateNode != null)
            {
                var dateStr = dateNode.GetAttributeValue("datetime", "");
                if (DateTime.TryParse(dateStr, out var parsed))
                    datePosted = parsed;
            }

            // 5. Salary (optional, not always present)
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
                SalaryMin = salaryMin,
                SalaryMax = salaryMax
            };
        }

        /// <summary>
        /// Fetches detailed job info from the job page: description, type, level, industry, etc.
        /// Also determines EasyApply (no external URL) and IsRemote (keyword detection).
        /// </summary>
        private async Task FetchJobDetails(Job job, bool? requestedRemote)
        {
            try
            {
                var response = await _httpClient.GetAsync(job.Url);
                if (!response.IsSuccessStatusCode)
                    return;

                // Check if redirected to signup page
                if (response.RequestMessage?.RequestUri?.ToString().Contains("linkedin.com/signup") == true)
                    return;

                var html = await response.Content.ReadAsStringAsync();

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Description
                var descNode = doc.DocumentNode.SelectSingleNode(
                    "//div[contains(@class, 'show-more-less-html__markup')]");
                if (descNode != null)
                    job.Description = descNode.InnerText.Trim();

                // Job criteria (type, level, industry, function)
                job.JobType = ExtractJobCriteria(doc, "Employment type");
                job.JobLevel = ExtractJobCriteria(doc, "Seniority level");
                job.Industry = ExtractJobCriteria(doc, "Industries");
                job.JobFunction = ExtractJobCriteria(doc, "Job function");

                // Apply URL (determines EasyApply)
                var applyNode = doc.DocumentNode.SelectSingleNode("//code[@id='applyUrl']");
                if (applyNode != null)
                {
                    var applyContent = applyNode.InnerText.Trim();
                    var match = Regex.Match(applyContent, @"\?url=([^""]+)");
                    if (match.Success)
                    {
                        job.DirectApplyUrl = HttpUtility.UrlDecode(match.Groups[1].Value);
                        job.EasyApply = false;
                    }
                    else
                    {
                        job.EasyApply = true;
                    }
                }
                else
                {
                    job.EasyApply = true;
                }

                // IsRemote: if user requested remote, use that; otherwise detect
                if (requestedRemote == true)
                {
                    job.IsRemote = true;
                }
                else
                {
                    job.IsRemote = DetectRemote(job.Title, job.Description, job.Location);
                }
            }
            catch
            {
                // Failed to fetch details, leave fields null
            }
        }

        /// <summary>
        /// Extracts job criteria from the details page (Employment type, Seniority level, Industries, Job function).
        /// </summary>
        private static string? ExtractJobCriteria(HtmlDocument doc, string criteriaName)
        {
            var headers = doc.DocumentNode.SelectNodes(
                "//h3[contains(@class, 'description__job-criteria-subheader')]");

            if (headers == null) return null;

            foreach (var header in headers)
            {
                if (header.InnerText.Contains(criteriaName))
                {
                    var valueNode = header.SelectSingleNode(
                        "following-sibling::span[contains(@class, 'description__job-criteria-text')]");
                    return valueNode?.InnerText.Trim();
                }
            }
            return null;
        }

        /// <summary>
        /// Detects if job is remote by searching for keywords in title, description, and location.
        /// </summary>
        private static bool DetectRemote(string? title, string? description, string? location)
        {
            var combined = $"{title} {description} {location}".ToLower();
            return RemoteKeywords.Any(keyword => combined.Contains(keyword));
        }

        /// <summary>
        /// Extracts job ID from the href (last segment after the last hyphen).
        /// Example: /jobs/view/senior-developer-123456 → 123456
        /// </summary>
        private static string? ExtractJobId(string href)
        {
            if (string.IsNullOrEmpty(href))
                return null;

            var cleanUrl = href.Split('?')[0];
            var parts = cleanUrl.Split('-');

            if (parts.Length == 0)
                return null;

            return parts[parts.Length - 1];
        }

        /// <summary>
        /// Parses salary range from text like "$80,000 - $120,000".
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
        /// Extracts numeric value from currency string.
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
        /// Example: https://www.linkedin.com/jobs-guest/jobs/api/seeMoreJobPostings/search?keywords=developer&amp;location=Vancouver
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
