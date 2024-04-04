using _2211_Final_Project_TGM_Blog.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.ComponentModel.DataAnnotations;

namespace _2211_Final_Project_TGM_Blog.Models.Admin
{
    public class UserAccounts {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public List<string> Role { get; set; }
        public List<string> RoleList { get; set; }
        public string Search { get; set; }

    }
}
