using _2211_Final_Project_TGM_Blog.Models.Admin;
using _2211_Final_Project_TGM_Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace _2211_Final_Project_TGM_Blog.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserAccountService _userAccountService;

        public AdminController(UserAccountService userAccountService)
        {
            _userAccountService = userAccountService;
        }

        [Route("admin/user-accounts")]
        public async Task<IActionResult> UserAccounts(string search)
        {
            if (!string.IsNullOrEmpty(search)) { 
                var userAccount = await _userAccountService.GetUserAccountDetailsAsync(search);
                if (userAccount == null)
                {
                    ViewData["ErrorMessage"] = "User not found.";
                    return View();
                }
                return View(userAccount);
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserAccounts model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userAccountService.UpdateUserAccountAsync(model);
                if (!result)
                {
                    ViewData["ErrorMessage"] = "User not found or update failed.";
                    return View("UserAccounts", model);
                }
                return RedirectToAction("UserAccounts", new { model.Search });
            }
            return View("UserAccounts", model);
        }
    }
}
