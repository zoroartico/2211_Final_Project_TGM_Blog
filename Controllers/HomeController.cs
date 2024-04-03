using _2211_Final_Project_TGM_Blog.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNet.Identity;

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
            //if (User.IsInRole == "Admin")
            return RedirectToAction("Register", "Account", new { area = "Identity" });
        }

        [Route("/admin/register")]
        public IActionResult RegisterAdmin()
        {
            var roles = RoleManager.Roles.Select(r => new SelectListItem { Value = r.Name, Text = r.Name }).ToList();
            ViewBag.Roles = roles; // Pass roles to the view bag
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
