using System.ComponentModel.DataAnnotations;

namespace _2211_Final_Project_TGM_Blog.Models.Forum
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation property
        public ICollection<ForumThread>? Threads { get; set; }
    }
}
