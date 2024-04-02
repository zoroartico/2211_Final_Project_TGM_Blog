using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.Blog;
using _2211_Final_Project_TGM_Blog.Services;
using System.Security.Claims;

namespace _2211_Final_Project_TGM_Blog.Controllers.Blog
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly UserRoleManager _userRoleManager;
        private readonly ILogger<BlogController> _logger;

        public BlogController(ApplicationDbContext context, 
            UserManager<IdentityUser> userManager, 
            UserRoleManager userRoleManager,
            ILogger<BlogController> logger)
        {
            _context = context;
            _userManager = userManager;
            _userRoleManager = userRoleManager;
            _logger = logger;
        }

        public async Task<IActionResult> MakeUserDev(string userId)
        {
            var result = await _userRoleManager.AddUserToRole(userId, "Dev");
            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Failed to make the user a Dev.");
                return RedirectToAction("Index");
            }
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Loading Index");
            var blogPosts = _context.BlogPosts.ToList();
            return View(blogPosts);
        }

        [Authorize]//(Roles = "Dev")] // Only users in "Dev" role can access this action
        [HttpGet]
        public IActionResult CreateBlogPost()
        {
            return View();
        }

        [Authorize]//(Roles = "Dev")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlogPost(BlogPost blogPost)
        {
            var user = await _userManager.GetUserAsync(User);
            blogPost.UserId = user.Id;
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Blog post created successfully.");
            return RedirectToAction(nameof(Index));
        }



        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            var blogPost = await _context.BlogPosts.FindAsync(postId);

            if (blogPost is null)
            {
                return NotFound();
            }

            Like like = new Like();
            like.UserId = user.Id;
            like.BlogPostId = blogPost.Id;
            like.BlogPost = blogPost;

            if (blogPost.Likes is null)
            {
                blogPost.Likes = new List<Like>();
            }

            blogPost.Likes.Add(like);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlike(int likeId)
        {
            _logger.LogInformation($"Unlike called likeId: {likeId}");
            var like = await _context.Likes.FindAsync(likeId);
            _logger.LogInformation($"Like: {like}");
            if (like is not null)
            {
                _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }

}
