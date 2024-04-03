using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using _2211_Final_Project_TGM_Blog.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace _2211_Final_Project_TGM_Blog.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Route("admin/user-accounts")]
        public async Task<IActionResult> UserAccounts(string search)
        {
            // If search parameter is provided, attempt to find the user
            if (!string.IsNullOrEmpty(search))
            {
                // Search for the user by username or email
                var user = await _userManager.FindByNameAsync(search) ?? await _userManager.FindByEmailAsync(search);

                if (user == null)
                {
                    // If user not found, return a view with appropriate message
                    ViewData["ErrorMessage"] = "User not found.";
                    return View();
                }

                // Get user roles
                var roles = await _userManager.GetRolesAsync(user);

                // Retrieve all roles from the database
                var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

                // Populate the view model with user information
                var userAccounts = new UserAccounts
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Role = roles, 
                    RoleList = allRoles,
                    Search = search
                };

                // Pass the user details to the view
                return View(userAccounts);
            }

            // If search parameter is not provided, return the view without populating user details
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Update(UserAccounts model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by username or email
                var user = await _userManager.FindByNameAsync(model.Username) ?? await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    // Update user's email if it has changed
                    if (user.Email != model.Email)
                    {
                        user.Email = model.Email;
                        var result = await _userManager.UpdateAsync(user);
                        if (!result.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(model);
                        }
                    }

                    // Update user's role if it has changed
                    var existingRoles = await _userManager.GetRolesAsync(user);
                    if (existingRoles != model.Role)
                    {
                        // Remove existing roles
                        foreach (var role in existingRoles)
                        {
                            await _userManager.RemoveFromRoleAsync(user, role);
                        }

                        // Add new role
                        await _userManager.AddToRoleAsync(user, model.Role.FirstOrDefault());
                    }

                    return RedirectToAction("UserAccounts",model.Search);
                }
                else
                {
                    ViewData["ErrorMessage"] = "User not found.";
                    return View("UserAccounts", model);
                }
            }

            // If model state is not valid, return the view with errors
            return View("UserAccounts", model);
        }

    }
}
