namespace _2211_Final_Project_TGM_Blog.Models.SupportChat
{
    public class SupportDashboardViewModel
    {
        public List<ChatRequest> PendingRequests { get; set; } = new List<ChatRequest>();
        public List<Chat> Chats { get; set; } = new List<Chat>();
    }
}
