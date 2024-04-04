using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using _2211_Final_Project_TGM_Blog.Controllers;
using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.SupportChat;

namespace _2211_Final_Project_TGM_Blog.Tests.UserSupportTests
{
    public class MessageControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly DefaultHttpContext _httpContext;

        public MessageControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "testUserId"),
                    new Claim(ClaimTypes.Role, "User")
                }, "TestAuthentication"))
            };
        }

        private async Task SeedDatabaseAsync()
        {
            var chat = new Chat { Id = 1, User1Id = "testUserId", User2Id = "anotherUserId", StartTime = DateTime.Now };
            await _context.Chats.AddAsync(chat);
            await _context.SaveChangesAsync();
        }

        // Chat Endpoint Tests
        [Fact]
        public async Task MessageController_Chat_ValidChatId_ReturnsViewWithChat()
        {
            // Arrange
            await SeedDatabaseAsync();
            var controller = new MessageController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
            var chatId = 1;

            // Act
            var result = await controller.Chat(chatId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Chat>(viewResult.Model);
            Assert.Equal(chatId, model.Id);
        }

        [Fact]
        public async Task MessageController_Chat_InvalidChatId_ReturnsNotFound()
        {
            // Arrange
            await SeedDatabaseAsync();
            var controller = new MessageController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
            var invalidChatId = 999;

            // Act
            var result = await controller.Chat(invalidChatId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        // Send Endpoint Tests
        [Fact]
        public async Task MessageController_Send_ValidData_RedirectsToChatDetails()
        {
            // Arrange
            await SeedDatabaseAsync(); // Seed database with a chat for sending messages to
            var controller = new MessageController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
            var chatId = 1;
            var content = "Test message content";

            // Act
            var result = await controller.Send(chatId, content);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirectToActionResult.ActionName);
            Assert.Equal("Chat", redirectToActionResult.ControllerName);
            Assert.Equal(chatId, redirectToActionResult.RouteValues["id"]);

            // Additional verification to ensure the message was added to the database
            var messageInDb = await _context.Messages.FirstOrDefaultAsync(m => m.ChatId == chatId && m.Content == content);
            Assert.NotNull(messageInDb);
            Assert.Equal("testUserId", messageInDb.SenderUserId);
        }
    }
}
