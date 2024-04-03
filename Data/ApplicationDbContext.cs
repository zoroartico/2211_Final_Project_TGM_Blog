using _2211_Final_Project_TGM_Blog.Models.Blog;
using _2211_Final_Project_TGM_Blog.Models.SupportChat;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace _2211_Final_Project_TGM_Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatRequest> ChatRequests { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BlogPost>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<BlogPost>()
                .HasMany(b => b.Likes)
                .WithOne(l => l.BlogPost)
                .HasForeignKey(l => l.BlogPostId);

            modelBuilder.Entity<Like>()
                .HasKey(l => l.Id);
        }
    }
}
