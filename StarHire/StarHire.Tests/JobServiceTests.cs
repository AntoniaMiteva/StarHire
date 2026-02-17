using AutoMapper;
using Moq;
using StarHire.Business.Repositories.Interfaces;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Jobs;
using StarHire.Services;
using System.Linq.Expressions;

namespace StarHire.Tests;

public class JobServiceTests
{
    private readonly Mock<IRepository<Job>> _mockJobRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly JobService _jobService;

    public JobServiceTests()
    {
        _mockJobRepository = new Mock<IRepository<Job>>();
        _mockMapper = new Mock<IMapper>();
        _jobService = new JobService(_mockJobRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetAll_WithNoFilters_ReturnsAllJobs()
    {
        // Arrange
        var jobs = new List<Job>
        {
            new Job { Id = Guid.NewGuid(), Title = "Developer", Planet = "Earth", Salary = 50000 },
            new Job { Id = Guid.NewGuid(), Title = "Designer", Planet = "Mars", Salary = 60000 }
        }.AsQueryable();

        var jobViewModels = new List<JobViewModel>
        {
            new JobViewModel { Title = "Developer", Planet = "Earth", Salary = 50000 },
            new JobViewModel { Title = "Designer", Planet = "Mars", Salary = 60000 }
        };

        _mockJobRepository.Setup(r => r.Query()).Returns(jobs);
        _mockMapper.Setup(m => m.Map<List<JobViewModel>>(It.IsAny<List<Job>>()))
            .Returns(jobViewModels);

        // Act
        var result = await _jobService.GetAll(null, null, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockJobRepository.Verify(r => r.Query(), Times.Once);
    }

    [Fact]
    public async Task GetAll_WithSearchFilter_ReturnsFilteredJobs()
    {
        
        var jobs = new List<Job>
        {
            new Job { Id = Guid.NewGuid(), Title = "Developer", Description = "Coding", Planet = "Earth", Salary = 50000 },
            new Job { Id = Guid.NewGuid(), Title = "Designer", Description = "Design", Planet = "Mars", Salary = 60000 }
        }.AsQueryable();

        var filteredJobs = new List<JobViewModel>
        {
            new JobViewModel { Title = "Developer", Description = "Coding", Planet = "Earth", Salary = 50000 }
        };

        _mockJobRepository.Setup(r => r.Query()).Returns(jobs);
        _mockMapper.Setup(m => m.Map<List<JobViewModel>>(It.IsAny<List<Job>>()))
            .Returns(filteredJobs);

        
        var result = await _jobService.GetAll("Developer", null, null);

        
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetAll_WithPlanetFilter_ReturnsFilteredJobs()
    {
        
        var jobs = new List<Job>
        {
            new Job { Id = Guid.NewGuid(), Title = "Developer", Planet = "Earth", Salary = 50000 },
            new Job { Id = Guid.NewGuid(), Title = "Designer", Planet = "Mars", Salary = 60000 }
        }.AsQueryable();

        var filteredJobs = new List<JobViewModel>
        {
            new JobViewModel { Title = "Developer", Planet = "Earth", Salary = 50000 }
        };

        _mockJobRepository.Setup(r => r.Query()).Returns(jobs);
        _mockMapper.Setup(m => m.Map<List<JobViewModel>>(It.IsAny<List<Job>>()))
            .Returns(filteredJobs);

        
        var result = await _jobService.GetAll(null, "Earth", null);

        
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetAll_WithMinSalaryFilter_ReturnsFilteredJobs()
    {
        
        var jobs = new List<Job>
        {
            new Job { Id = Guid.NewGuid(), Title = "Developer", Planet = "Earth", Salary = 50000 },
            new Job { Id = Guid.NewGuid(), Title = "Designer", Planet = "Mars", Salary = 60000 }
        }.AsQueryable();

        var filteredJobs = new List<JobViewModel>
        {
            new JobViewModel { Title = "Designer", Planet = "Mars", Salary = 60000 }
        };

        _mockJobRepository.Setup(r => r.Query()).Returns(jobs);
        _mockMapper.Setup(m => m.Map<List<JobViewModel>>(It.IsAny<List<Job>>()))
            .Returns(filteredJobs);

        
        var result = await _jobService.GetAll(null, null, 55000);

        
        Assert.NotNull(result);
        Assert.Single(result);
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
            Salary = 50000
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
        _mockMapper.Setup(m => m.Map<JobViewModel>(It.IsAny<Job>()))
            .Returns(jobViewModel);

       
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
            Salary = 50000
        };

        var job = new Job
        {
            Title = "Developer",
            Description = "Coding",
            Planet = "Earth",
            Salary = 50000
        };

        _mockMapper.Setup(m => m.Map<Job>(model)).Returns(job);
        _mockJobRepository.Setup(r => r.AddAsync(It.IsAny<Job>())).Returns(Task.CompletedTask);
        _mockJobRepository.Setup(r => r.CommitAsync()).Returns(Task.FromResult(0));

        
        await _jobService.Create(model, employerId);

       
        _mockMapper.Verify(m => m.Map<Job>(model), Times.Once);
        _mockJobRepository.Verify(r => r.AddAsync(It.Is<Job>(j => j.EmployeerId == employerId)), Times.Once);
        _mockJobRepository.Verify(r => r.CommitAsync(), Times.Once);
    }
}