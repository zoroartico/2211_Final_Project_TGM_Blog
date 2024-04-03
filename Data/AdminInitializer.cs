using Microsoft.AspNetCore.Identity;

namespace _2211_Final_Project_TGM_Blog.Data
{
    public class AdminInitializer
    {
        public static async Task CreateUsers(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userName = "admin";
                var email = "admin@admin.admin";
                var password = "admin";

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var userCheck = await userManager.FindByEmailAsync(email);
                if (userCheck == null)
                {
                    var user = new IdentityUser { UserName = userName, Email = email , PasswordHash = password};
                    await userManager.CreateAsync(user);
                    await userManager.AddToRoleAsync(user, "Admin");
                }
                
            }
        }

    }
}
