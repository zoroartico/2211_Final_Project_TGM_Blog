﻿using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.SupportChat;

namespace _2211_Final_Project_TGM_Blog.Controllers
{
    [Authorize]
    public class ChatRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ChatRequest/SupportDashboard
        public async Task<IActionResult> SupportDashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var viewModel = new SupportDashboardViewModel
            {
                Chats = await _context.Chats
                    .Where(chat => chat.User1Id == userId || chat.User2Id == userId)
                    .OrderByDescending(chat => chat.StartTime)
                    .ToListAsync()
            };

            if (User.IsInRole("Agent"))
            {
                viewModel.PendingRequests = await _context.ChatRequests
                    .Where(cr => cr.Status == "Pending")
                    .ToListAsync();
            }

            return View(viewModel);
        }

        // POST: /ChatRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Create a new chat request with no receiver specified
            var chatRequest = new ChatRequest
            {
                SenderUserId = userId,
                Status = "Pending"
            };

            _context.ChatRequests.Add(chatRequest);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Chat Request Created! An Agent will be with you shortly.";

            return RedirectToAction(nameof(SupportDashboard));
        }

        // POST: /ChatRequest/Accept/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var chatRequest = await _context.ChatRequests.FindAsync(id);
            if (chatRequest == null)
            {
                return NotFound();
            }

            // Create a new chat with the sender of the request and the agent
            var newChat = new Chat
            {
                User1Id = chatRequest.SenderUserId,
                User2Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                StartTime = DateTime.Now
            };

            _context.Chats.Add(newChat);
            await _context.SaveChangesAsync();

            // Update the status of the chat request to indicate it has been accepted
            chatRequest.Status = "Accepted";
            _context.Update(chatRequest);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(SupportDashboard));
        }
    }
}