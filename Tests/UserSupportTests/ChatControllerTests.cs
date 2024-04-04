using _2211_Final_Project_TGM_Blog.Controllers;
using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.SupportChat;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

namespace _2211_Final_Project_TGM_Blog.Tests.UserSupportTests
{
    public class ChatControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly DefaultHttpContext _httpContext;

        public ChatControllerTests()
        {
            // Configure the DbContext to use an in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            // Setup HttpContext to simulate User Identity
            _httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.NameIdentifier, "userId"),
                    }, "TestAuthType"))
            };
        }
        private async Task SeedDatabaseAsync()
        {
            var chat = new Chat { Id = 1, User1Id = "userId", User2Id = "user2", StartTime = DateTime.Now, Messages = new List<Message>() };
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
        }

        // Details Endpoint Test Cases
        [Fact]
        public async Task ChatController_Details_NullId_ReturnsNotFound()
        {
            // Arrange
            var controller = new ChatController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = await controller.Details(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ChatController_Details_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var controller = new ChatController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = await controller.Details(999); // Assuming ID 999 does not exist

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ChatController_Details_ValidId_ReturnsViewResultWithChat()
        {
            // Arrange
            await SeedDatabaseAsync(); // Ensure the database is seeded with the necessary data before testing
            var controller = new ChatController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = await controller.Details(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Chat>(viewResult.Model);
            Assert.Equal(1, model.Id);
        }

        // Start Endpoint Test Cases
        [Fact]
        public async Task ChatController_Start_WithUserId_CreatesNewChatAndRedirects()
        {
            // Arrange
            var controller = new ChatController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
            var userId = "user2Id"; // ID of the other user involved in the chat

            // Act
            var result = await controller.Start(userId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Verifying the correct user ID is assigned
            var chatInDb = await _context.Chats.FirstOrDefaultAsync(c => c.User2Id == userId);
            Assert.NotNull(chatInDb);
            Assert.Equal("userId", chatInDb.User1Id);
        }

        [Fact]
        public async Task ChatController_Start_NullUserId_ReturnsNotFound()
        {
            // Arrange
            var controller = new ChatController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = await controller.Start(null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // End Endpoint Test Cases
        [Fact]
        public async Task ChatController_End_ExistingChat_EndsChatAndRedirects()
        {
            // Arrange
            await SeedDatabaseAsync();
            var controller = new ChatController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
            var chatId = 1;

            // Act
            var result = await controller.End(chatId);

            // Assert
            var chat = await _context.Chats.FindAsync(chatId);
            Assert.NotNull(chat.EndTime);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task ChatController_End_InvalidChatId_ReturnsNotFound()
        {
            // Arrange
            await SeedDatabaseAsync();
            var controller = new ChatController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
            var invalidChatId = 999;

            // Act
            var result = await controller.End(invalidChatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }

}