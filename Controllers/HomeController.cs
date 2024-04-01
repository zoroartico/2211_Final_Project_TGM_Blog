using _2211_Final_Project_TGM_Blog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace _2211_Final_Project_TGM_Blog.Controllers
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
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Login()
        {
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        public IActionResult Register()
        {
            return RedirectToAction("Register", "Account", new { area = "Identity" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
