using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.Forum;
using System.Threading.Tasks;
using _2211_Final_Project_TGM_Blog.Services;
using Microsoft.AspNetCore.Identity;

namespace _2211_Final_Project_TGM_Blog.Controllers.Forum
{
    [Authorize]
    public class ForumThreadController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ForumThreadController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public ForumThreadController(ApplicationDbContext context, ILogger<ForumThreadController> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // Details Action
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Accessing details for thread ID: {id}");

            var forumThread = await _context.ForumThreads
                                            .Include(t => t.Posts)
                                            .ThenInclude(p => p.User)
                                            .AsNoTracking()
                                            .FirstOrDefaultAsync(t => t.Id == id);

            if (forumThread == null)
            {
                _logger.LogWarning($"Thread with ID: {id} not found.");
                return NotFound();
            }

            var postViewModels = new List<PostViewModel>();

            foreach (var post in forumThread.Posts)
            {
                var user = await _userManager.FindByIdAsync(post.UserId.ToString());
                var username = user?.UserName;

                postViewModels.Add(new PostViewModel
                {
                    Id = post.Id,
                    Content = post.Content,
                    CreatedAt = post.CreatedAt,
                    ImageData = post.ImageData,
                    Username = username
                });
            }

            var viewModel = new ForumThreadViewModel
            {
                Id = forumThread.Id,
                Title = forumThread.Title,
                CreatedAt = forumThread.CreatedAt,
                Posts = postViewModels
            };

            return View(viewModel);
        }

        // Create Action
        [Authorize(Roles ="Admin")]
        public IActionResult Create(int categoryId)
        {
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