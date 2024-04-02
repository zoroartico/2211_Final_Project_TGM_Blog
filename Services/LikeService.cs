using _2211_Final_Project_TGM_Blog.Data;
using Microsoft.EntityFrameworkCore;

public class LikeService
{
    private readonly ApplicationDbContext _context;

    public LikeService(ApplicationDbContext context)
    {
        _context = context;
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

    public async Task<bool> HasUserLikedPost(int postId, string userId)
    {
        var like = await _context.Likes
            .FirstOrDefaultAsync(l => l.BlogPostId == postId && l.UserId == userId);

        return like != null;
    }
}
