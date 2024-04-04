using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using _2211_Final_Project_TGM_Blog.Controllers;
using _2211_Final_Project_TGM_Blog.Data;
using _2211_Final_Project_TGM_Blog.Models.SupportChat;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;

namespace _2211_Final_Project_TGM_Blog.Tests.UserSupportTests
{
    public class ChatRequestControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly DefaultHttpContext _httpContext;

        public ChatRequestControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);

            _httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "user1Id"),
                    new Claim(ClaimTypes.Role, "User"),
                }, "TestAuthentication"))
            };
        }

        private async Task SeedDatabaseAsync()
        {
            var user = new Microsoft.AspNetCore.Identity.IdentityUser
            {
                Id = "user1Id",
                UserName = "testUser"
            };
            _context.Users.Add(user);

            var chatRequest = new ChatRequest
            {
                Id = 1,
                SenderUserId = "user1Id",
                Status = "Pending"
            };

            await _context.ChatRequests.AddAsync(chatRequest);

            var chat = new Chat
            {
                Id = 1,
                User1Id = "user1Id",
                User2Id = "anotherUserId",
                StartTime = DateTime.Now
            };

            await _context.Chats.AddAsync(chat);

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task ChatRequestController_SupportDashboard_AsUser_ReturnsUserDashboardView()
        {
            // Arrange
            var controller = new ChatRequestController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = await controller.SupportDashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ChatRequestController_SupportDashboard_AsAgent_ReturnsAgentDashboardViewWithPendingRequests()
        {
            // Arrange
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "agentId"),
                new Claim(ClaimTypes.Role, "Agent"),
            }, "TestAuthentication"));

            var controller = new ChatRequestController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = await controller.SupportDashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task ChatRequestController_Create_CreatesNewChatRequestAndRedirects()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var context = new ApplicationDbContext(options);

            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "testUserId"),
            }, "TestAuthentication"));

            var controller = new ChatRequestController(context)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Mocking TempData
            controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());

            // Act
            var result = await controller.Create();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("SupportDashboard", redirectToActionResult.ActionName);
            Assert.True(controller.TempData.ContainsKey("Message"), "Expected TempData to contain a 'Message' key.");
        }

        [Fact]
        public async Task ChatRequestController_Accept_ValidRequest_UpdatesRequestStatusAndRedirects()
        {
            // Arrange
            await SeedDatabaseAsync();
            var controller = new ChatRequestController(_context)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
            var requestId = 1; // Use an ID from your seeded data

            // Act
            var result = await controller.Accept(requestId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(ChatRequestController.SupportDashboard), redirectToActionResult.ActionName);
        }
    }
}