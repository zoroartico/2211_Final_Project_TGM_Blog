using System.ComponentModel.DataAnnotations;

namespace _2211_Final_Project_TGM_Blog.Models.Blog
{
    public class BlogPost
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public string UserId { get; set; } //foreign key from ApplicationUser
        public ICollection<Like>? Likes { get; set; }
    }
}
