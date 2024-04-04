
namespace _2211_Final_Project_TGM_Blog.Models.Forum
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageData { get; set; }
        public string Username { get; set; }
    }
}
