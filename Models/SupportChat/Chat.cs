using System.ComponentModel.DataAnnotations;

namespace _2211_Final_Project_TGM_Blog.Models.SupportChat
{
    public class Chat
    {
        public int Id { get; set; }

        [Required]
        public string User1Id { get; set; }

        [Required]
        public string User2Id { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        // Navigation properties for EF Core
        public virtual ICollection<Message> Messages { get; set; }
    }
}
