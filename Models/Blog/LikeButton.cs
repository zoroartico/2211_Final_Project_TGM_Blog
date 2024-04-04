namespace _2211_Final_Project_TGM_Blog.Models.Blog
{
    //simple model used for processing like button view data
    public class LikeButtonModel
    {
        public bool IsAuthenticated { get; set; }
        public bool UserLikedPost { get; set; }
        public int LikeId { get; set; }
        public int PostId { get; set; }
        public int LikeQTY { get; set; }
    }
}
