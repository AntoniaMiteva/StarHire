using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using StarHire.Business.Repositories.Interfaces;
using StarHire.Data;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Jobs;
using StarHire.Services;

namespace StarHire.Tests;

public class JobServiceTests
{
    private readonly Mock<IRepository<Job>> _mockJobRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ApplicationDbContext _db;
    private readonly JobService _jobService;

    public JobServiceTests()
    {
        _mockJobRepository = new Mock<IRepository<Job>>();
        _mockMapper = new Mock<IMapper>();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _db = new ApplicationDbContext(options);

        _jobService = new JobService(_mockJobRepository.Object, _mockMapper.Object, _db);
    }

    [Fact]
    public async Task GetAll_WithNoFilters_ReturnsAllJobs()
    {
        _db.Jobs.AddRange(
            new Job { Id = Guid.NewGuid(), Title = "Developer", Planet = "Earth", Salary = 50000, EmployerId = Guid.NewGuid() },
            new Job { Id = Guid.NewGuid(), Title = "Designer", Planet = "Mars", Salary = 60000, EmployerId = Guid.NewGuid() }
        );
        await _db.SaveChangesAsync();

        var result = await _jobService.GetAll(null, null, null);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetAll_WithSearchFilter_ReturnsFilteredJobs()
    {
        _db.Jobs.AddRange(
            new Job { Id = Guid.NewGuid(), Title = "Developer", Description = "Coding", Planet = "Earth", Salary = 50000, EmployerId = Guid.NewGuid() },
            new Job { Id = Guid.NewGuid(), Title = "Designer", Description = "Design", Planet = "Mars", Salary = 60000, EmployerId = Guid.NewGuid() }
        );
        await _db.SaveChangesAsync();

        var result = await _jobService.GetAll("Developer", null, null);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Developer", result[0].Title);
    }

    [Fact]
    public async Task GetAll_WithPlanetFilter_ReturnsFilteredJobs()
    {
        _db.Jobs.AddRange(
            new Job { Id = Guid.NewGuid(), Title = "Developer", Planet = "Earth", Salary = 50000, EmployerId = Guid.NewGuid() },
            new Job { Id = Guid.NewGuid(), Title = "Designer", Planet = "Mars", Salary = 60000, EmployerId = Guid.NewGuid() }
        );
        await _db.SaveChangesAsync();

        var result = await _jobService.GetAll(null, "Earth", null);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Earth", result[0].Planet);
    }

    [Fact]
    public async Task GetAll_WithMinSalaryFilter_ReturnsFilteredJobs()
    {
        _db.Jobs.AddRange(
            new Job { Id = Guid.NewGuid(), Title = "Developer", Planet = "Earth", Salary = 50000, EmployerId = Guid.NewGuid() },
            new Job { Id = Guid.NewGuid(), Title = "Designer", Planet = "Mars", Salary = 60000, EmployerId = Guid.NewGuid() }
        );
        await _db.SaveChangesAsync();

        var result = await _jobService.GetAll(null, null, 55000);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Designer", result[0].Title);
    }

    [Fact]
    public async Task GetById_WithValidId_ReturnsJobViewModel()
    {
        var jobId = Guid.NewGuid();
        var job = new Job
        {
            Id = jobId,
            Title = "Developer",
            Description = "Coding",
            Planet = "Earth",
            Salary = 50000,
            EmployerId = Guid.NewGuid()
        };

        var jobViewModel = new JobViewModel
        {
            Id = jobId,
            Title = "Developer",
            Description = "Coding",
            Planet = "Earth",
            Salary = 50000
        };

        var jobs = new List<Job> { job }.AsQueryable();
        _mockJobRepository.Setup(r => r.Query()).Returns(jobs);
        _mockMapper.Setup(m => m.Map<JobViewModel>(It.IsAny<Job>())).Returns(jobViewModel);

        var result = await _jobService.GetById(jobId);

        Assert.NotNull(result);
        Assert.Equal(jobId, result.Id);
        Assert.Equal("Developer", result.Title);
    }

    [Fact]
    public async Task GetById_WithInvalidId_ReturnsNull()
    {
        var jobId = Guid.NewGuid();
        var jobs = new List<Job>().AsQueryable();
        _mockJobRepository.Setup(r => r.Query()).Returns(jobs);

        var result = await _jobService.GetById(jobId);

        Assert.Null(result);
    }

    [Fact]
    public async Task Create_WithValidModel_CreatesJob()
    {
        var employerId = Guid.NewGuid();
        var model = new CreateJobViewModel
        {
            Title = "Developer",
            Description = "Coding",
            Planet = "Earth",
            Salary = 50000,
            SelectedSkillIds = new List<Guid>()
        };

        await _jobService.Create(model, employerId);

        var job = _db.Jobs.FirstOrDefault(j => j.Title == "Developer");
        Assert.NotNull(job);
        Assert.Equal(employerId, job.EmployerId);
        Assert.Equal("Earth", job.Planet);
    }
}