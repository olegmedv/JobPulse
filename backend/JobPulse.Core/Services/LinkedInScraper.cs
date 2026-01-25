using JobPulse.Core.Models;
using JobPulse.Core.Scrapers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobPulse.Core.Services
{
    public class LinkedInScraper : IJobScraper
    {
        private readonly HttpClient _httpClient;
        public LinkedInScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public string Source => throw new NotImplementedException();

        public Task<List<Job>> ScrapeAsync(SearchRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
