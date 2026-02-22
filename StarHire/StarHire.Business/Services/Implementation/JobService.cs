using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarHire.Business.Repositories.Interfaces;
using StarHire.Business.Services.Interfaces;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Applications;
using StarHire.Models.ViewModels.Jobs;
using StarHire.Models.ViewModels.Applications;


namespace StarHire.Services;

public class JobService : IJobService
{
    private readonly IRepository<Job> _jobRepository;
    private readonly IMapper _mapper;

    public JobService(IRepository<Job> jobRepository, IMapper mapper)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
    }

    public async Task<List<JobViewModel>> GetAll(string? search, string? planet, decimal? minSalary)
    {
        var query = _jobRepository.Query();

       
        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(j => j.Title.Contains(search) || j.Description.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(planet))
        {
            query = query.Where(j => j.Planet == planet);
        }

        if (minSalary.HasValue)
        {
            query = query.Where(j => j.Salary >= minSalary.Value);
        }

        var jobs = await query
            .Include(j => j.Employeer)
            .ToListAsync();

        return _mapper.Map<List<JobViewModel>>(jobs);
    }

    public async Task<JobViewModel?> GetById(Guid id)
    {
        var job = await _jobRepository.Query()
            .Include(j => j.Employeer)
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null)
        {
            return null;
        }

        return _mapper.Map<JobViewModel>(job);
    }

    public async Task Create(CreateJobViewModel model, Guid employerId)
    {
        var job = _mapper.Map<Job>(model);
        job.EmployeerId = employerId;

        await _jobRepository.AddAsync(job);
        await _jobRepository.CommitAsync();
    }


    public async Task<List<JobViewModel>> GetByEmployer(Guid employerId)
    {
        var jobs = await _jobRepository.Query()
            .Where(j => j.EmployeerId == employerId)
            .Include(j => j.Applications)
                .ThenInclude(a => a.Alien)
            .ToListAsync();

        return _mapper.Map<List<JobViewModel>>(jobs);
    }

    public async Task<List<ApplicantViewModel>> GetApplicants(Guid jobId, Guid employerId)
    {
        var job = await _jobRepository.Query()
            .Include(j => j.Applications)
                .ThenInclude(a => a.Alien)
            .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployeerId == employerId);

        if (job == null) return new List<ApplicantViewModel>();

        return job.Applications.Select(a => new ApplicantViewModel
        {
            ApplicationId = a.Id,
            AlienId = a.AlienId,
            AlienEmail = a.Alien.Email!,
            AlienUserName = a.Alien.UserName!,
            Status = a.Status.ToString()
        }).ToList();
    }

}