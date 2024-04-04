namespace _2211_Final_Project_TGM_Blog.Models.SupportChat
{
    public class SupportDashboardViewModel
    {
        public IEnumerable<ChatViewModel>? Chats { get; set; }
        public IEnumerable<ChatRequest>? PendingRequests { get; set; }
    }
}
