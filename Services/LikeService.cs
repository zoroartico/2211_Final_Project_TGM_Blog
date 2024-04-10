using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.Blog;
using Microsoft.EntityFrameworkCore;

public class LikeService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<LikeService> _logger;

    //constructing the LikeService class
    public LikeService(ApplicationDbContext context, ILogger<LikeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    //function that processes a like being sent to the database
    public virtual async Task Like(string userId, int blogPostId, BlogPost blogPost)
    {
        var existingLike = await _context.Likes
            .FirstOrDefaultAsync(like => like.UserId == userId && like.BlogPostId == blogPostId);

        if (existingLike != null)
        {
            _logger.LogError("Like already exists.");
            return;
        }

        if (userId is not null && blogPost is not null)
        {
            blogPost.Likes ??= new List<Like>();
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


    //process for removing a like from the database
    public virtual async Task Unlike(int likeId)
    {
        var like = await _context.Likes.FindAsync(likeId);
        if (like is not null)
        {
            _context.Likes.Remove(like);
            await _context.SaveChangesAsync(); //if the like is found, removes it successfully
        }
    }

    //funcion that handles getting all the likes for a post
    public virtual async Task<int> GetLikes(int postId)
    {
        return await _context.Likes //returns the number of likes found
            .Where(like => like.BlogPostId == postId)
            .CountAsync();
    }

    //function that gets an amount of likes specific to a blogpost like via its id
    public virtual async Task<int> GetLikesByLikeId(int likeId)
    {
        var postId = await GetPostIdByLikeId(likeId);
        return await GetLikes(postId);

    }

    //function that checks if the post has any likes
    public virtual async Task<bool> HasLikes(int postId)
    {
        if (await GetLikes(postId) > 0)
        {
            return true;
        }
        return false;
    }

    //gets a specific like by the postId and userId combination key
    public virtual async Task<Like?> GetLike(int postId, string userId)
    {
        return await _context.Likes
            .FirstOrDefaultAsync(l => l.BlogPostId == postId && l.UserId == userId);
    }

    //gets a like from the database by its id
    public virtual async Task<Like?> GetLikeById(int likeId)
    {
        return await _context.Likes
            .FirstOrDefaultAsync(l => l.Id == likeId);
    }

    //gets the id of a post a like is attached to via the post's id
    public virtual async Task<int> GetPostIdByLikeId(int likeId)
    {
        var like = await GetLikeById(likeId);
        if (like is not null) { return like.BlogPostId; }
        return 0;
    }

    //gets the id of a like by the postId and userId combination key 
    public virtual async Task<int> GetLikeId(int postId, string userId)
    {
        var like = await GetLike(postId, userId) ?? throw new Exception("Like not found.");
        return like.Id;
    }

    //checks if a user has liked a post by the postId and userId combination key
    public virtual async Task<bool> HasUserLikedPost(int postId, string userId)
    {
        var like = await GetLike(postId, userId);
        return like != null;
    }
}
