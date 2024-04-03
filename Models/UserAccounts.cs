using Microsoft.AspNetCore.Mvc.Rendering;

namespace _2211_Final_Project_TGM_Blog.Models
{
    public class UserAccounts
    {
        string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public IList<String> Role { get; set; }
        public List<String> RoleList { get; set; }
        public string Search { get; set; }

    }
}
