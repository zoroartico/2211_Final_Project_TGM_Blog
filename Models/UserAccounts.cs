using Microsoft.AspNetCore.Mvc.Rendering;

namespace _2211_Final_Project_TGM_Blog.Models
{
    public class UserAccounts
    {
        string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public string Search { get; set; }

    }
}
