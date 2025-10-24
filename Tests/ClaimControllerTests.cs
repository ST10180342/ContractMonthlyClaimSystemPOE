using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContractMonthlyClaimSystemPOE.Controllers;
using ContractMonthlyClaimSystemPOE.Data;
using ContractMonthlyClaimSystemPOE.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Claim = ContractMonthlyClaimSystemPOE.Models.Claim;

namespace ContractMonthlyClaimSystemPOE.Tests
{
    public class ClaimsControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly ClaimsController _controller;

        public ClaimsControllerTests()
        {
            _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            _mockUserManager = CreateUserManagerMock();
            _controller = new ClaimsController(_mockContext.Object, _mockUserManager.Object);
        }

        private Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
        {
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var optionsMock = new Mock<IOptions<IdentityOptions>>();
            var passwordHasherMock = new Mock<IPasswordHasher<ApplicationUser>>();
            var userValidatorsMock = new Mock<IUserValidator<ApplicationUser>>[] { };
            var passwordValidatorsMock = new Mock<IPasswordValidator<ApplicationUser>>[] { };
            var lookupNormalizerMock = new Mock<ILookupNormalizer>();
            var errorDescriberMock = new Mock<IdentityErrorDescriber>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            var loggerMock = new Mock<ILogger<UserManager<ApplicationUser>>>();

            return new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, optionsMock.Object, passwordHasherMock.Object,
                userValidatorsMock, passwordValidatorsMock, lookupNormalizerMock.Object, errorDescriberMock.Object,
                serviceProviderMock.Object, loggerMock.Object);
        }

        private void SetupAuthContext(string role = "Lecturer")
        {
            var claims = new System.Security.Claims.Claim[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "testuser"),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, role)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            ClaimsPrincipal user = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Submit_ValidClaim_ReturnsRedirect()
        {
            // Arrange
            SetupAuthContext("Lecturer");
            var claim = new Claim { HoursWorked = 40, HourlyRate = 250 };
            _mockContext.Setup(c => c.Claims.Add(It.IsAny<Claim>()));
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.Submit(claim, null) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("MyClaims", result.ActionName);
        }

        [Fact]
        public async Task Submit_InvalidHours_AddsModelError()  // Now async
        {
            // Arrange
            SetupAuthContext("Lecturer");
            var claim = new Claim { HoursWorked = -1, HourlyRate = 250 };

            // Act
            var result = await _controller.Submit(claim, null) as ViewResult;  // Awaited

            // Assert
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey("HoursWorked"));
        }

        [Fact]
        public async Task Approve_ValidId_UpdatesStatus()
        {
            // Arrange
            SetupAuthContext("Coordinator");
            var claim = new Claim { Id = 1, Status = "Pending" };
            _mockContext.Setup(c => c.Claims.FindAsync(1)).ReturnsAsync(claim);
            _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await _controller.Approve(1, "Approve") as RedirectToActionResult;

            // Assert
            Assert.Equal("Approved", claim.Status);
            Assert.Equal("PendingClaims", result.ActionName);
        }

    }
}