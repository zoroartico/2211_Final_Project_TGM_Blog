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

    public async Task<int> GetLikes(int postId)
    {
        return await _context.Likes
            .Where(like => like.BlogPostId == postId)
            .CountAsync();
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
