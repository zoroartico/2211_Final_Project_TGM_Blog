using Microsoft.AspNetCore.Mvc;
using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.Forum;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using _2211_Final_Project_TGM_Blog.Models.Blog;
using Microsoft.AspNetCore.Identity;

namespace _2211_Final_Project_TGM_Blog.Controllers.Forum
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PostController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly UserManager<IdentityUser> _userManager;

        public PostController(ApplicationDbContext context, ILogger<PostController> logger, IWebHostEnvironment hostingEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _userManager = userManager;
        }

        // Create Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Content,ThreadId,UserId")] Post post, IFormFile image)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                post.UserId = userId;
                var user = await _userManager.FindByIdAsync(userId);
                post.User = user;
                post.CreatedAt = DateTime.Now;

                // Process the uploaded image
                if (image != null && image.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", uniqueFileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    post.ImageData = "/images/" + uniqueFileName;
                }
                _context.Add(post);
                await _context.SaveChangesAsync();
                if (post.ImageData != null)
                {
                    _logger.LogInformation(post.ImageData);
                }
                else
                {
                    _logger.LogWarning("No Image");
                }

                _logger.LogInformation($"Successfully created a new post: {post.Content}");

                return RedirectToAction("Details", "ForumThread", new { id = post.ThreadId });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Attempt to create a Post failed due to invalid model state. Errors: {ModelStateErrors}", GetModelStateErrors());
                _logger.LogError(ex, "Error creating blog post.");
            }
            ViewBag.ThreadId = post.ThreadId;
            return RedirectToAction("Details", "ForumThread", new { id = post.ThreadId });
        }
        private string GetModelStateErrors()
        {
            return string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

        // Delete Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                _logger.LogWarning($"Attempted to delete a post that does not exist. Post ID: {id}");
                return NotFound();
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Successfully deleted post. Post ID: {id}");
            return RedirectToAction("Details", "ForumThread", new { id = post.ThreadId });
        }
    }
}