using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace _2211_Final_Project_TGM_Blog.Models.Forum
{
    public class ForumThread
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Creation date is required")]
        public DateTime CreatedAt { get; set; }

        // Foreign keys
        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }


        // Navigation propertys
        public Category? Category { get; set; }
        public ICollection<Post>? Posts { get; set; }
    }
}
