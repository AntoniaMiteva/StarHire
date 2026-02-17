using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarHire.Business.Repositories.Interfaces;
using StarHire.Business.Services.Interfaces;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Jobs;


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
}