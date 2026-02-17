using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using StarHire.Business.Services.Interfaces;
using StarHire.Controllers;
using StarHire.Models.ViewModels.Applications;
using System.Security.Claims;

namespace StarHire.Tests;

public class ApplicationsControllerTests
{
    private readonly Mock<IApplicationService> _mockApplicationService;
    private readonly ApplicationsController _controller;

    public ApplicationsControllerTests()
    {
        _mockApplicationService = new Mock<IApplicationService>();
        _controller = new ApplicationsController(_mockApplicationService.Object);

        // Setup TempData
        _controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>()
        );
    }

    private void SetupUserClaims(Guid userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task Apply_WithValidData_RedirectsToDetailsWithSuccessMessage()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var message = "I am interested";

        SetupUserClaims(userId);

        _mockApplicationService.Setup(s => s.ApplyAsync(jobId, userId, message))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Apply(jobId, message);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal("Jobs", redirectResult.ControllerName);
        Assert.Equal(jobId, redirectResult.RouteValues?["id"]);
        Assert.Equal("You have successfully applied for this position!", _controller.TempData["SuccessMessage"]);
        _mockApplicationService.Verify(s => s.ApplyAsync(jobId, userId, message), Times.Once);
    }

    [Fact]
    public async Task Apply_WhenAlreadyApplied_RedirectsToDetailsWithErrorMessage()
    {
       
        var userId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var message = "I am interested";

        SetupUserClaims(userId);

        _mockApplicationService.Setup(s => s.ApplyAsync(jobId, userId, message))
            .ThrowsAsync(new InvalidOperationException("You have already applied for this job."));

        
        var result = await _controller.Apply(jobId, message);

        
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirectResult.ActionName);
        Assert.Equal("Jobs", redirectResult.ControllerName);
        Assert.Equal("You have already applied for this job.", _controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task Apply_WhenExceptionOccurs_RedirectsToDetailsWithGenericErrorMessage()
    {
        
        var userId = Guid.NewGuid();
        var jobId = Guid.NewGuid();
        var message = "I am interested";

        SetupUserClaims(userId);

        _mockApplicationService.Setup(s => s.ApplyAsync(jobId, userId, message))
            .ThrowsAsync(new Exception("Database error"));

        
        var result = await _controller.Apply(jobId, message);

        
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("An error occurred while applying.", _controller.TempData["ErrorMessage"]);
    }

    [Fact]
    public async Task Apply_WithNullMessage_UsesEmptyString()
    {
        
        var userId = Guid.NewGuid();
        var jobId = Guid.NewGuid();

        SetupUserClaims(userId);

        _mockApplicationService.Setup(s => s.ApplyAsync(jobId, userId, string.Empty))
            .Returns(Task.CompletedTask);

        
        var result = await _controller.Apply(jobId, null);

        
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        _mockApplicationService.Verify(s => s.ApplyAsync(jobId, userId, string.Empty), Times.Once);
    }

    [Fact]
    public async Task MyApplications_ReturnsViewWithApplications()
    {
       
        var userId = Guid.NewGuid();
        SetupUserClaims(userId);

        var applications = new List<ApplicationViewModel>
        {
            new ApplicationViewModel { JobId = Guid.NewGuid() },
            new ApplicationViewModel { JobId = Guid.NewGuid() }
        };

        _mockApplicationService.Setup(s => s.GetMyApplicationsAsync(userId))
            .ReturnsAsync(applications);

        
        var result = await _controller.MyApplications();

        
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ApplicationViewModel>>(viewResult.Model);
        Assert.Equal(2, model.Count);
        _mockApplicationService.Verify(s => s.GetMyApplicationsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task MyApplications_WithNoApplications_ReturnsEmptyList()
    {
       
        var userId = Guid.NewGuid();
        SetupUserClaims(userId);

        var applications = new List<ApplicationViewModel>();

        _mockApplicationService.Setup(s => s.GetMyApplicationsAsync(userId))
            .ReturnsAsync(applications);

        
        var result = await _controller.MyApplications();

        
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<ApplicationViewModel>>(viewResult.Model);
        Assert.Empty(model);
    }
}
