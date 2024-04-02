using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.Blog;
using Microsoft.EntityFrameworkCore;

public class LikeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LikeService> _logger;

    public LikeService(ApplicationDbContext context, ILogger<LikeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Like(string userId, int blogPostId, BlogPost blogPost)
    {
        foreach (Like like in _context.Likes)
        {
            if (like.UserId == userId && like.BlogPostId == blogPostId) 
            {
                _logger.LogError("Like already exists.");
                return; 
            }
        }
        if (userId is not null && blogPost is not null)
        {
            blogPost.Likes ??= [];
            var newLike = new Like
            {
                UserId = userId,
                BlogPostId = blogPostId,
                BlogPost = blogPost
            };
            blogPost.Likes.Add(newLike);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Unlike(int likeId)
    {
        var like = await _context.Likes.FindAsync(likeId);
        if (like is not null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetLikes(int postId)
    {
        return await _context.Likes
            .Where(like => like.BlogPostId == postId)
            .CountAsync();
    }

    public async Task<int> GetLikesByLikeId(int likeId)
    {
        var like = await _context.Likes.FindAsync(likeId);
        if (like != null)
        {
            return await _context.Likes.CountAsync(l => l.BlogPostId == like.BlogPostId);
        }
        return 0;
    }

    public async Task<bool> HasLikes(int postId)
    {
        if (await GetLikes(postId) > 0)
        {
            return true;
        }
        return false;
    }

    public async Task<Like?> GetLike(int postId, string userId)
    {
        return await _context.Likes
            .FirstOrDefaultAsync(l => l.BlogPostId == postId && l.UserId == userId);
    }

    public async Task<int> GetLikeId(int postId, string userId)
    {
        _logger.LogInformation("Get Like Id Called.");
        _logger.LogInformation($"postId: {postId} userId: {userId}");
        var like = await GetLike(postId, userId);
        _logger.LogInformation($"like: {like}");
        _logger.LogInformation($"likeId: {like.Id}");
        return like.Id;
    }

    public async Task<bool> HasUserLikedPost(int postId, string userId)
    {
        var like = await GetLike(postId, userId);
        return like != null;
    }
}
