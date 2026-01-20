using Microsoft.AspNetCore.Mvc;

namespace JobPulse.Api.Controllers
{
    public class JobsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
