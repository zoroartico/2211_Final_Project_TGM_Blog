using System.ComponentModel.DataAnnotations;

namespace _2211_Final_Project_TGM_Blog.Models.SupportChat
{
    public class ChatRequest
    {
        public int Id { get; set; }

        [Required]
        public string SenderUserId { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; }
    }
}
