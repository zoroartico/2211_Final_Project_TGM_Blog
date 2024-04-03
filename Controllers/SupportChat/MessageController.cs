using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2211_Final_Project_TGM_Blog.Models.SupportChat;
using System;
using System.Linq;
using System.Threading.Tasks;
using _2211_Final_Project_TGM_Blog.Data;

namespace _2211_Final_Project_TGM_Blog.Controllers
{
    public class MessageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessageController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Message/Chat/{id}
        public async Task<IActionResult> Chat(int id)
        {
            var chat = await _context.Chats
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        // POST: /Message/Send
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(int ChatId, string Content)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            string senderRole = User.IsInRole("Agent") ? "Agent" : User.IsInRole("User") ? "User" : "Unknown";

            if (!string.IsNullOrWhiteSpace(Content))
            {
                var message = new Message
                {
                    ChatId = ChatId,
                    SenderUserId = userId,
                    SenderRole = senderRole,
                    Content = Content,
                    TimeSent = DateTime.Now
                };

                _context.Messages.Add(message);
                await _context.SaveChangesAsync();

                // Redirect to the ChatController's Details action/view for the current chat
                return RedirectToAction("Details", "Chat", new { id = ChatId });
            }
            return RedirectToAction("Details", "Chat", new { id = ChatId, error = "Message content cannot be empty." });
        }
    }
}