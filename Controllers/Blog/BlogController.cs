using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.Blog;

namespace _2211_Final_Project_TGM_Blog.Controllers.Blog
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public BlogController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var blogPosts = _context.BlogPosts.ToList();
            return View(blogPosts);
        }

        [Authorize(Roles = "Dev")] // Only users in "Dev" role can access this action
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Dev")] // Only users in "Dev" role can access this action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPost blogPost)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                blogPost.UserId = user.Id;
                _context.BlogPosts.Add(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blogPost);
        }

        [Authorize] // Any authenticated user can like a post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            var like = new Like
            {
                UserId = user.Id,
                BlogPostId = postId
            };
            _context.Likes.Add(like);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }

}
