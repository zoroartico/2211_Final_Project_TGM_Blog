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
        private readonly IWebHostEnvironment _hostingEnvironment;

        //constructing the BlogController class
        public BlogController(ApplicationDbContext context, 
            UserManager<IdentityUser> userManager, 
            UserRoleManager userRoleManager,
            ILogger<BlogController> logger,
            LikeService likeService,
            IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _userRoleManager = userRoleManager;
            _logger = logger;
            _likeService = likeService;
            _hostingEnvironment = hostingEnvironment;
        }

        //function that utilizes the _userRoleManager to give a user a "Dev" role
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

        //function that utilizes the _userRoleManager to remove all roles from a user
        public async Task<IActionResult> MakeUserUser(string userId)
        {
            var result = await _userRoleManager.SetDefaultRole(userId);
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

        //index method, loads when entering the blog controller
        public IActionResult Index()
        {
            _logger.LogInformation("Loading Index");
            var blogPosts = _context.BlogPosts.ToList();
            return View(blogPosts);
        }

        //method onluy accessible to "Dev"s to create a blog post
        [Authorize (Roles = "Dev")] //only users in "Dev" role can access this action
        [HttpGet]
        public IActionResult CreateBlogPost()
        {
            return View();
        }

        //method that posts a created blogpost, only accessible to the "Dev" role
        [Authorize (Roles = "Dev")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBlogPost(BlogPost blogPost, IFormFile imageFile)
        {
            try
            {
                //adds an image file if there is one
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    var imagePath = Path.Combine(_hostingEnvironment.WebRootPath, "images", uniqueFileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    blogPost.ImageUrl = "/images/" + uniqueFileName;
                }

                blogPost.UserId = _userManager.GetUserId(User);

                _logger.LogInformation($"Blog Post information" +
                    $"Title: {blogPost.Title}" +
                    $"Content: {blogPost.Content}" +
                    $"ImgURL: {blogPost.ImageUrl}" +
                    $"UserID: {blogPost.UserId}");

                _context.BlogPosts.Add(blogPost);
                await _context.SaveChangesAsync();
                //redirects to index upon completion
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //returns the create view if there are exceptions
                _logger.LogError(ex, "Error creating blog post.");
                return View(blogPost);
            }
        }

        //method only accessible to "Dev"s that deletes a given blog post
        [Authorize (Roles = "Dev")]
        [HttpPost]
        public async Task<IActionResult> Delete(int postId)
        {
            try
            {
                var blogPost = _context.BlogPosts.FirstOrDefault(b => b.Id == postId);
                _context.BlogPosts.Remove(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blog post.");
            }
            return RedirectToAction(nameof(Index));
        }

        //method that handles returning a partial view when a user likes a post
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(int postId)
        {
            var user = await _userManager.GetUserAsync(User);
            var blogPost = await _context.BlogPosts.FindAsync(postId);
            await _likeService.Like(user.Id, postId, blogPost);
            var model = await GetLikeButtonModel(postId);
            return PartialView("_LikeButtonPartial", model);
        }

        //method that returns a partial view upon a user unliking a post
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlike(int likeId)
        {
            int postId = await _likeService.GetPostIdByLikeId(likeId);
            await _likeService.Unlike(likeId);
            var model = await GetLikeButtonModel(postId);
            return PartialView("_LikeButtonPartial", model);
        }

        //method that handles generation of the LikeButton model in a given view
        private async Task<LikeButtonModel> GetLikeButtonModel(int postId)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool userLikedPost = await _likeService.HasUserLikedPost(postId, currentUser.Id);
            int likeId = userLikedPost ? await _likeService.GetLikeId(postId, currentUser.Id) : -1;
            int likeQty = await _likeService.GetLikesByLikeId(likeId);
            return new LikeButtonModel //returns a model with generated fields
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserLikedPost = userLikedPost,
                LikeId = likeId,
                PostId = postId,
                LikeQTY = likeQty
            };
        }

    }

}
