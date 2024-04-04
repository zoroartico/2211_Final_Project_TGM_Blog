using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.Forum;
using System.Threading.Tasks;

namespace _2211_Final_Project_TGM_Blog.Controllers.Forum
{
    [Authorize]
    public class ForumThreadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ForumThreadController> _logger;

        public ForumThreadController(ApplicationDbContext context, ILogger<ForumThreadController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Details Action
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Accessing details for thread ID: {id}");
            var forumThread = await _context.ForumThreads
                                            .Include(t => t.Posts)
                                            .FirstOrDefaultAsync(t => t.Id == id);

            if (forumThread == null)
            {
                _logger.LogWarning($"Thread with ID: {id} not found.");
                return NotFound();
            }

            return View(forumThread);
        }

        // Create Action
        [Authorize(Roles ="Admin")]
        public IActionResult Create(int categoryId)
        {
            // Optionally, pass the CategoryId to the view to pre-select the category or use it in the view logic
            ViewBag.CategoryId = categoryId;
            _logger.LogInformation("Showing the Create Thread form.");
            return View();
        }

        // Create Action pt 2
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,CategoryId")] ForumThread forumThread)
        {
            if (ModelState.IsValid)
            {
                forumThread.CreatedAt = DateTime.Now;
                _context.Add(forumThread);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully created a new thread: {forumThread.Title}");

                return RedirectToAction("Details", "Category", new { id = forumThread.CategoryId });
            }
            ViewBag.CategoryId = forumThread.CategoryId;
            return View(forumThread);
        }

        // Delete Action
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumThread = await _context.ForumThreads
                                            .Include(t => t.Posts)
                                            .FirstOrDefaultAsync(t => t.Id == id);
            if (forumThread == null)
            {
                _logger.LogWarning($"Attempted to delete a thread that does not exist. Thread ID: {id}");
                return NotFound();
            }
            // Deletes the messages associated with the thread
            if (forumThread.Posts != null && forumThread.Posts.Any())
            {
                _context.Posts.RemoveRange(forumThread.Posts);
            }
            // Deletes the thread
            _context.ForumThreads.Remove(forumThread);
            // Saves changes to the database
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Successfully deleted thread and its associated messages. Thread ID: {id}");
            return RedirectToAction("Details", "Category", new { id = forumThread.CategoryId });
        }
    }
}