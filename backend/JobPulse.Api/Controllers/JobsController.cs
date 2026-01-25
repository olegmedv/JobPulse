using JobPulse.Core.Models;
using JobPulse.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPulse.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly JobService _jobService;

        /// <summary>
        /// Constructor. JobService is injected via DI.
        /// </summary>
        public JobsController(JobService jobService)
        {
            _jobService = jobService;
        }


        [HttpPost("search")]
        public async Task<ActionResult<List<Job>>> Search([FromBody] SearchRequest request)
        {
            // Execute search
            var jobs = await _jobService.SearchAsync(request);

            return Ok(jobs);
        }
    }
}
