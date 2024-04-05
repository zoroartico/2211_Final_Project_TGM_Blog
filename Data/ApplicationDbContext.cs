using _2211_Final_Project_TGM_Blog.Models.Blog;
using _2211_Final_Project_TGM_Blog.Models.Forum;
using _2211_Final_Project_TGM_Blog.Models.SupportChat;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _2211_Final_Project_TGM_Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public readonly object AspNetUsers;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatRequest> ChatRequests { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BlogPost>()
                .HasKey(b => b.Id); //blog posts have a primary key: Id

            modelBuilder.Entity<BlogPost>()
                .HasMany(b => b.Likes) //a blog post can have many likes
                .WithOne(l => l.BlogPost) //a like can only have one blog post
                .HasForeignKey(l => l.BlogPostId); //likes have a blog post Id as a foreign key

            modelBuilder.Entity<Like>()
                .HasKey(l => l.Id); //likes have a primary key: Id
        }
    }
}
