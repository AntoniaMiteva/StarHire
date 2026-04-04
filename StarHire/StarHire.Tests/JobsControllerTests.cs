using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using StarHire.Business.Services.Interfaces;
using StarHire.Controllers;
using StarHire.Models.ViewModels.Jobs;
using System.Security.Claims;

namespace StarHire.Tests;

public class JobsControllerTests
{
    private readonly Mock<IJobService> _mockJobService;
    private readonly Mock<IFavoriteService> _mockFavoriteService;
    private readonly JobsController _controller;

    public JobsControllerTests()
    {
        _mockJobService = new Mock<IJobService>();
        _mockFavoriteService = new Mock<IFavoriteService>();
        _controller = new JobsController(_mockJobService.Object, _mockFavoriteService.Object);

        // Setup default user context
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task Index_WithNoFilters_ReturnsViewWithAllJobs()
    {
        var jobs = new List<JobViewModel>
        {
            new JobViewModel { Title = "Developer", Planet = "Earth", Salary = 50000 },
            new JobViewModel { Title = "Designer", Planet = "Mars", Salary = 60000 }
        };

        _mockJobService.Setup(s => s.GetAll(null, null, null, null))
            .ReturnsAsync(jobs);

        var result = await _controller.Index(null, null, null);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<JobViewModel>>(viewResult.Model);
        Assert.Equal(2, model.Count);
    }

    [Fact]
    public async Task Index_WithFilters_ReturnsFilteredJobs()
    {
        var jobs = new List<JobViewModel>
        {
            new JobViewModel { Title = "Developer", Planet = "Earth", Salary = 50000 }
        };

        _mockJobService.Setup(s => s.GetAll("Developer", "Earth", 40000, null))
            .ReturnsAsync(jobs);

        var result = await _controller.Index("Developer", "Earth", 40000);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<JobViewModel>>(viewResult.Model);
        Assert.Single(model);
        Assert.Equal("Developer", _controller.ViewBag.Search);
        Assert.Equal("Earth", _controller.ViewBag.Planet);
        Assert.Equal(40000m, _controller.ViewBag.MinSalary);
    }

    [Fact]
    public async Task Details_WithValidId_ReturnsViewWithJob()
    {
        var jobId = Guid.NewGuid();
        var job = new JobViewModel
        {
            Id = jobId,
            Title = "Developer",
            Planet = "Earth",
            Salary = 50000
        };

        _mockJobService.Setup(s => s.GetById(jobId)).ReturnsAsync(job);

        var result = await _controller.Details(jobId);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<JobViewModel>(viewResult.Model);
        Assert.Equal(jobId, model.Id);
    }

    [Fact]
    public async Task Details_WithInvalidId_ReturnsNotFound()
    {
        var jobId = Guid.NewGuid();
        _mockJobService.Setup(s => s.GetById(jobId)).ReturnsAsync((JobViewModel?)null);

        var result = await _controller.Details(jobId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Create_Get_ReturnsView()
    {
        _mockJobService.Setup(s => s.GetAllSkillsAsync()).ReturnsAsync(new List<StarHire.Models.ViewModels.Skills.SkillCheckboxViewModel>());

        var result = await _controller.Create();

        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task Create_Post_WithValidModel_RedirectsToIndex()
    {
        var employerId = Guid.NewGuid();
        var model = new CreateJobViewModel
        {
            Title = "Developer",
            Description = "Coding",
            Planet = "Earth",
            Salary = 50000
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, employerId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockJobService.Setup(s => s.Create(model, employerId)).Returns(Task.CompletedTask);

        var result = await _controller.Create(model);

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task Create_Post_WithInvalidModel_ReturnsViewWithModel()
    {
        var model = new CreateJobViewModel();
        _controller.ModelState.AddModelError("Title", "Required");

        var result = await _controller.Create(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Same(model, viewResult.Model);
        _mockJobService.Verify(s => s.Create(It.IsAny<CreateJobViewModel>(), It.IsAny<Guid>()), Times.Never);
    }
}