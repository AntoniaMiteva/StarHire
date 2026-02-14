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

    public async Task ApplyAsync(Guid jobId, string userId, string message)
    {
        var alreadyApplied = await _applicationRepository.Query()
            .AnyAsync(a => a.JobId == jobId && a.AlienId == userId);

        if (alreadyApplied)
        {
            throw new InvalidOperationException("Вече си кандидатствал за тази обява.");
        }

        //TODO: var application = _mapper.Map<Application>(viewmodel)
        var application =  new Application
            {
                JobId = jobId,
                AlienId = userId,
                Status = ApplicationStatus.Pending
            };

        await _applicationRepository.AddAsync(application);
        await _applicationRepository.CommitAsync();
    }

    public async Task<List<ApplicationViewModel>> GetMyApplicationsAsync(string userId)
    {
        var applications = await _applicationRepository.Query()
            .Include(a => a.Job)
            .Where(a => a.AlienId == userId)
            .ToListAsync();

        return _mapper.Map<List<ApplicationViewModel>>(applications);
    }
}
