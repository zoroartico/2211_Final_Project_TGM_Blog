using System.ComponentModel.DataAnnotations;
using Xunit.Sdk;

namespace _2211_Final_Project_TGM_Blog.Models.Blog
{
    public class BlogPost
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Content is required")]
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public string UserId { get; set; } //foreign key from ApplicationUser
        public ICollection<Like>? Likes { get; set; }
    }
}
