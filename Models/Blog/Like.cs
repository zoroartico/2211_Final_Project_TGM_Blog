namespace _2211_Final_Project_TGM_Blog.Models.Blog
{
    public class Like
    {
        public int Id { get; set; }
        public string UserId { get; set; } //foreign key for ApplicationUser
        public int BlogPostId { get; set; } //foreign key for BlogPost
        public BlogPost BlogPost { get; set; }
    }
}
