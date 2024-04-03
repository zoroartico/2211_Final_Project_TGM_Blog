using System.ComponentModel.DataAnnotations;

namespace _2211_Final_Project_TGM_Blog.Models.SupportChat
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string SenderUserId { get; set; }

        [Required]
        public int ChatId { get; set; }

        [Required]
        public string SenderRole { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime TimeSent { get; set; }

        // Navigation property for EF Core
        public virtual Chat Chat { get; set; }
    }
}
