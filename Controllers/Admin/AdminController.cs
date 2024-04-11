using _2211_Final_Project_TGM_Blog.Models.Admin;
using _2211_Final_Project_TGM_Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

            //if search is null, return view without error, user has just loaded int the page
            if (!string.IsNullOrEmpty(search)) {
                search = search.Trim();
                var userAccount = await _userAccountService.GetUserAccountDetailsAsync(search);
                //if return is null, return view with error, user has searched invalid user
                if (userAccount != null)
                {
                    //Successful user search
                    return View(userAccount);
                }
                //set error message for invalid search
                TempData["ErrorMessage"] = "User not found.";
            }
            //On load returns base view
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserAccounts model)
        {
            // Initialize TempData to avoid null reference error
            TempData ??= new TempDataDictionary(HttpContext, _tempDataProvider);

            if (!ModelState.IsValid)
            {
                //retrieve all error messages from ModelState
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                //creates string with error list items, must be displayed with @html.Raw(errorMessage) or else <br> will be displayed as string
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
            //Redirects back to user account page with last user updated already in partial view
            return RedirectToAction("UserAccounts", new { model.Search });
        }
    }
}
