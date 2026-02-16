using AutoMapper;
using Moq;
using StarHire.Business.Repositories.Interfaces;
using StarHire.Models;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Applications;
using StarHire.Services;

namespace StarHire.Tests;

public class ApplicationServiceTests
{
    private readonly Mock<IRepository<Application>> _mockApplicationRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ApplicationService _applicationService;

    public ApplicationServiceTests()
    {
        _mockApplicationRepository = new Mock<IRepository<Application>>();
        _mockMapper = new Mock<IMapper>();
        _applicationService = new ApplicationService(_mockApplicationRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task ApplyAsync_WithValidData_CreatesApplication()
    {
        // Arrange
        var jobId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var message = "I am interested in this position";

        var applications = new List<Application>().AsQueryable();
        _mockApplicationRepository.Setup(r => r.Query()).Returns(applications);

        var application = new Application
        {
            JobId = jobId,
            AlienId = userId,
            Status = ApplicationStatus.Pending
        };

        _mockMapper.Setup(m => m.Map<Application>(It.IsAny<CreateApplicationViewModel>()))
            .Returns(application);
        _mockApplicationRepository.Setup(r => r.AddAsync(It.IsAny<Application>()))
            .Returns(Task.CompletedTask);
        _mockApplicationRepository.Setup(r => r.CommitAsync())
            .Returns(Task.FromResult(0));

        // Act
        await _applicationService.ApplyAsync(jobId, userId, message);

        // Assert
        _mockMapper.Verify(m => m.Map<Application>(It.IsAny<CreateApplicationViewModel>()), Times.Once);
        _mockApplicationRepository.Verify(r => r.AddAsync(It.IsAny<Application>()), Times.Once);
        _mockApplicationRepository.Verify(r => r.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ApplyAsync_WhenAlreadyApplied_ThrowsInvalidOperationException()
    {
        
        var jobId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var message = "I am interested in this position";

        var existingApplication = new Application
        {
            JobId = jobId,
            AlienId = userId,
            Status = ApplicationStatus.Pending
        };

        var applications = new List<Application> { existingApplication }.AsQueryable();
        _mockApplicationRepository.Setup(r => r.Query()).Returns(applications);

        
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _applicationService.ApplyAsync(jobId, userId, message)
        );

        Assert.Equal("You have already applied for this job.", exception.Message);
        _mockApplicationRepository.Verify(r => r.AddAsync(It.IsAny<Application>()), Times.Never);
        _mockApplicationRepository.Verify(r => r.CommitAsync(), Times.Never);
    }

    [Fact]
    public async Task GetMyApplicationsAsync_WithValidUserId_ReturnsApplications()
    {
       
        var userId = Guid.NewGuid();
        var jobId1 = Guid.NewGuid();
        var jobId2 = Guid.NewGuid();

        var applications = new List<Application>
        {
            new Application
            {
                Id = Guid.NewGuid(),
                JobId = jobId1,
                AlienId = userId,
                Status = ApplicationStatus.Pending,
                Job = new Job { Id = jobId1, Title = "Developer" }
            },
            new Application
            {
                Id = Guid.NewGuid(),
                JobId = jobId2,
                AlienId = userId,
                Status = ApplicationStatus.Approved,
                Job = new Job { Id = jobId2, Title = "Designer" }
            }
        }.AsQueryable();

        var applicationViewModels = new List<ApplicationViewModel>
        {
            new ApplicationViewModel { JobId = jobId1, Status = ApplicationStatus.Pending },
            new ApplicationViewModel { JobId = jobId2, Status = ApplicationStatus.Approved }
        };

        _mockApplicationRepository.Setup(r => r.Query()).Returns(applications);
        _mockMapper.Setup(m => m.Map<List<ApplicationViewModel>>(It.IsAny<List<Application>>()))
            .Returns(applicationViewModels);

        
        var result = await _applicationService.GetMyApplicationsAsync(userId);

       
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockApplicationRepository.Verify(r => r.Query(), Times.Once);
    }

    [Fact]
    public async Task GetMyApplicationsAsync_WithNoApplications_ReturnsEmptyList()
    {
      
        var userId = Guid.NewGuid();
        var applications = new List<Application>().AsQueryable();

        _mockApplicationRepository.Setup(r => r.Query()).Returns(applications);
        _mockMapper.Setup(m => m.Map<List<ApplicationViewModel>>(It.IsAny<List<Application>>()))
            .Returns(new List<ApplicationViewModel>());

        
        var result = await _applicationService.GetMyApplicationsAsync(userId);

        
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}