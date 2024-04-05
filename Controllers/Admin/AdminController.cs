using _2211_Final_Project_TGM_Blog.Models.Admin;
using _2211_Final_Project_TGM_Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace _2211_Final_Project_TGM_Blog.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserAccountService _userAccountService;
        private readonly ITempDataProvider _tempDataProvider;


        public AdminController(UserAccountService userAccountService, ITempDataProvider tempDataProvider)
        {
            _userAccountService = userAccountService;
            _tempDataProvider = tempDataProvider;
        }

        [Route("admin/user-accounts")]
        public async Task<IActionResult> UserAccounts(string search)
        {
            // Initialize TempData
            TempData ??= new TempDataDictionary(HttpContext, _tempDataProvider);

            if (!string.IsNullOrEmpty(search)) { 
                var userAccount = await _userAccountService.GetUserAccountDetailsAsync(search);
                if (userAccount != null)
                {
                    return View(userAccount);
                }
                //set error message for invalid user
                TempData["ErrorMessage"] = "User not found.";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserAccounts model)
        {
            // Initialize TempData
            TempData ??= new TempDataDictionary(HttpContext, _tempDataProvider);

            if (!ModelState.IsValid)
            {
                //retrieve all error messages from ModelState
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                //creates string with error list items, must be displayed with @html.Raw(~~~) or else <br> will be displayed as string
                var errorMessage = string.Join("<br>", errorMessages);

                //set error message for invalid data being supplied for update
                TempData["ErrorMessage"] = errorMessage;
            }
            else {
                //attempt update data
                var result = await _userAccountService.UpdateUserAccountAsync(model);
                if (!result)
                {
                    //set error message for unable to update user data
                    TempData["ErrorMessage"] = "Unexpected error. Update failed";
                }
            }
            return RedirectToAction("UserAccounts", new { model.Search });
        }
    }
}
