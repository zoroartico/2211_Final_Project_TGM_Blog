using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace _2211_Final_Project_TGM_Blog.Models.Forum
{
    public class Post
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters")]
        public string Content { get; set; }

        public string? ImageData { get; set; }

        [Required(ErrorMessage = "Creation date is required")]
        public DateTime CreatedAt { get; set; }

        // Foreign keys
        [Required(ErrorMessage = "Thread ID is required")]
        public int ThreadId { get; set; }

        [Required]
        public string UserId { get; set; }

        // Navigation propertys
        public ForumThread? Thread { get; set; }
        public IdentityUser? User { get; set; }
    }
}
