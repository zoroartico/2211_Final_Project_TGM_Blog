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
        private readonly LikeService _likeService;

        public BlogController(ApplicationDbContext context, 
            UserManager<IdentityUser> userManager, 
            UserRoleManager userRoleManager,
            ILogger<BlogController> logger,
            LikeService likeService)
        {
            _context = context;
            _userManager = userManager;
            _userRoleManager = userRoleManager;
            _logger = logger;
            _likeService = likeService;
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
                _logger.LogError("Failed to make the user a Dev.");
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> MakeUserUser(string userId)
        {
            var result = await _userRoleManager.AddUserToRole(userId, "User");
            if (result)
            {
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogError("Failed to make the user a User.");
                return RedirectToAction("Index");
            }
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Loading Index");
            var blogPosts = _context.BlogPosts.ToList();
            return View(blogPosts);
        }

        [Authorize]//(Roles = "Dev")] //only users in "Dev" role can access this action
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

            await _likeService.Like(user.Id, postId, blogPost);

            int likesCount = await _likeService.GetLikes(postId);

            return Json(new { likesCount });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlike(int likeId)
        {
            await _likeService.Unlike(likeId);

            int likesCount = await _likeService.GetLikesByLikeId(likeId);

            return Json(new { likesCount });
        }

    }

}
