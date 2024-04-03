namespace _2211_Final_Project_TGM_Blog.Services
{
    using Microsoft.AspNetCore.Identity;

    public class UserRoleManager
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserRoleManager> _logger;
        private string defaultRole = "User";

        public UserRoleManager(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserRoleManager> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<bool> SetDefaultRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Get all roles for the user
                var userRoles = await _userManager.GetRolesAsync(user);

                // Remove all roles except the default role
                foreach (var role in userRoles)
                {
                    if (role != defaultRole)
                    {
                        await _userManager.RemoveFromRoleAsync(user, role);
                    }
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddUserToRole(string userId, string roleName)
        {
            _logger.LogInformation($"Attempting to convert {userId} to role: {roleName}");
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                _logger.LogInformation("Not Null");
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await Console.Out.WriteLineAsync("Creating undefined role...");
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                var result = await _userManager.AddToRoleAsync(user, roleName);
                _logger.LogInformation("Conversion successful.");
                return result.Succeeded;
            }
            return false;
        }
    }
}
