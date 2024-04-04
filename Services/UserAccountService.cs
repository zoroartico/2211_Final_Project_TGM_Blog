using _2211_Final_Project_TGM_Blog.Models.Admin;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2211_Final_Project_TGM_Blog.Services
{
    public class UserAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserRoleManager _userRoleManager;

        public UserAccountService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, UserRoleManager userRoleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRoleManager = userRoleManager;
        }

        public async Task<UserAccounts> GetUserAccountDetailsAsync(string search)
        {
            if (string.IsNullOrEmpty(search))
                return null;

            var user = await _userManager.FindByNameAsync(search) ?? await _userManager.FindByEmailAsync(search);

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || !roles.Any())
                await _userRoleManager.SetDefaultRole(user.Id);
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

            return new UserAccounts
            {
                Username = user.UserName,
                Email = user.Email,
                Role = (List<string>)roles,
                RoleList = allRoles,
                Search = search
            };
        }

        public async Task<bool> UpdateUserAccountAsync(UserAccounts model)
        {
            var user = await _userManager.FindByNameAsync(model.Username) ?? await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return false;

            if (user.UserName != model.Username)
            {
                user.UserName = model.Username;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return false;
                model.Search = user.UserName;
            }

            if (user.Email != model.Email)
            {
                user.Email = model.Email;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                    return false;
            }

            var existingRoles = await _userManager.GetRolesAsync(user);
            if (existingRoles != model.Role)
            {
                foreach (var role in existingRoles)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
                foreach (var role in model.Role) 
                {
                    await _userRoleManager.AddUserToRole(user.Id, role);
                }
            }

            return true;
        }
    }
}
