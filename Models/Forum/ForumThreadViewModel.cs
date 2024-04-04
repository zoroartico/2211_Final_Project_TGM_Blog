namespace _2211_Final_Project_TGM_Blog.Models.Forum
{
    public class ForumThreadViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PostViewModel>? Posts { get; set; }
    }
}
