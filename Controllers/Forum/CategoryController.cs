using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.Forum;

namespace _2211_Final_Project_TGM_Blog.Controllers.Forum
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Index Action
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Accessed Category Index");
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // Details Action
        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation($"Accessing details for category ID: {id}");
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                _logger.LogWarning($"Category with ID: {id} not found.");
                return NotFound();
            }

            var threads = await _context.ForumThreads.Where(t => t.CategoryId == id).ToListAsync();
            var viewModel = new CategoryDetailsViewModel { Category = category, Threads = threads };

            return View(viewModel);
        }

        // Create Action
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("Showing the Create Category form.");
            return View();
        }

        // Create Action pt. 2
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description")] Category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Successfully created a new category: {CategoryName}", category.Name);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating category: {CategoryName}", category.Name);
                }
            }
            else
            {
                _logger.LogWarning("Attempt to create a category failed due to invalid model state. Errors: {ModelStateErrors}", GetModelStateErrors());
            }

            return View(category);
        }

        private string GetModelStateErrors()
        {
            return string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        }

        // Delete Action
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.Include(c => c.Threads).ThenInclude(t => t.Posts).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                _logger.LogWarning($"Attempted to delete a category that does not exist. Category ID: {id}");
                return NotFound();
            }

            try
            {
                // Deleting Posts and threads associated with the category
                foreach (var thread in category.Threads)
                {
                    _context.Posts.RemoveRange(thread.Posts);
                }
                _context.ForumThreads.RemoveRange(category.Threads);

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully deleted category and its associated threads and messages. Category ID: {id}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category with ID: {id}");
                throw;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}