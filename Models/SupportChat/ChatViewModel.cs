namespace _2211_Final_Project_TGM_Blog.Models.SupportChat
{
    public class ChatViewModel
    {
        public int Id { get; set; }

        public string User1Id { get; set; }
        public string User2Id { get; set; }

        // Usernames for display
        public string User1Username { get; set; }
        public string User2Username { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        // Simplified view model for messages, if needed
        public IEnumerable<MessageViewModel> Messages { get; set; }
    }
}
