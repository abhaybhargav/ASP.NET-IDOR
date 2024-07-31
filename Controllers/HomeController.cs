using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IDORDemoApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Home Controller Index action called");
            return View();
        }
    }
}