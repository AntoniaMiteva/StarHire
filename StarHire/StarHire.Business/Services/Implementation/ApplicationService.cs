using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarHire.Business.Repositories.Interfaces;
using StarHire.Business.Services.Interfaces;
using StarHire.Data;
using StarHire.Models;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Applications;

namespace StarHire.Services;

public class ApplicationService : IApplicationService
{
    private readonly IRepository<Application> _applicationRepository;
    private readonly IMapper _mapper;

    public ApplicationService(IRepository<Application> applicationRepository, IMapper mapper)
    {
        _applicationRepository = applicationRepository;
        _mapper = mapper;
    }

    public async Task ApplyAsync(Guid jobId, Guid userId, string message)
    {
        var alreadyApplied = await _applicationRepository.Query()
            .AnyAsync(a => a.JobId == jobId && a.AlienId == userId);

        if (alreadyApplied)
        {
            throw new InvalidOperationException("Вече си кандидатствал за тази обява.");
        }

        
        var viewModel = new CreateApplicationViewModel
        {
            JobId = jobId,
            AlienId = userId,
            Message = message,
            Status = ApplicationStatus.Pending
        };

        // Map viewmodel to Application entity
        var application = _mapper.Map<Application>(viewModel);

        await _applicationRepository.AddAsync(application);
        await _applicationRepository.CommitAsync();
    }

    public async Task<List<ApplicationViewModel>> GetMyApplicationsAsync(Guid userId)
    {
        var applications = await _applicationRepository.Query()
            .Include(a => a.Job)
            .Where(a => a.AlienId == userId)
            .ToListAsync();

        return _mapper.Map<List<ApplicationViewModel>>(applications);
    }
}