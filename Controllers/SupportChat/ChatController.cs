using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.SupportChat;
using System.Security.Claims;

namespace _2211_Final_Project_TGM_Blog.Controllers
{
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint to display details of a specific chat
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chat = await _context.Chats
                .Include(chat => chat.Messages)
                .FirstOrDefaultAsync(chat => chat.Id == id);

            if (chat == null)
            {
                return NotFound();
            }

            return View(chat);
        }

        // Endpoint to start a new chat
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var chat = new Chat
            {
                User1Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                User2Id = userId,
                StartTime = DateTime.Now
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Endpoint to end an existing chat
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> End(int id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            chat.EndTime = DateTime.Now;

            _context.Chats.Update(chat);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}