using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StarHire.Business.Repositories.Interfaces;
using StarHire.Business.Services.Interfaces;
using StarHire.Data;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Applications;
using StarHire.Models.ViewModels.Jobs;
using StarHire.Models.ViewModels.Skills;

namespace StarHire.Services;

public class JobService : IJobService
{
    private readonly IRepository<Job> _jobRepository;
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _db;

    public JobService(IRepository<Job> jobRepository, IMapper mapper, ApplicationDbContext db)
    {
        _jobRepository = jobRepository;
        _mapper = mapper;
        _db = db;
    }

    public async Task<List<SkillCheckboxViewModel>> GetAllSkillsAsync()
    {
        return await _db.Skills
            .OrderBy(s => s.Name)
            .Select(s => new SkillCheckboxViewModel { Id = s.Id, Name = s.Name })
            .ToListAsync();
    }

    public async Task<IEnumerable<JobViewModel>> GetRecommendedAsync(Guid userId)
    {
        var userSkillIds = await _db.UserSkills
            .Where(us => us.UserId == userId)
            .Select(us => us.SkillId)
            .ToListAsync();

        var jobs = await _db.Jobs
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .Where(j => j.JobSkills.Any(js => userSkillIds.Contains(js.SkillId)))
            .ToListAsync();

        return jobs.Select(j => new JobViewModel
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            Salary = j.Salary,
            Planet = j.Planet,
            EmployerId = j.EmployerId,
            RequiredSkills = j.JobSkills.Select(js => js.Skill.Name).ToList(),
            CompatibilityPercent = j.JobSkills.Any()
                ? (int)Math.Round((double)j.JobSkills.Count(js => userSkillIds.Contains(js.SkillId)) / j.JobSkills.Count * 100)
                : null
        }).ToList();
    }

    public async Task<List<JobViewModel>> GetAll(string? search, string? planet, decimal? minSalary, Guid? userId = null)
    {
        var query = _db.Jobs
            .Include(j => j.Applications)
            .Include(j => j.JobSkills)
                .ThenInclude(js => js.Skill)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(j => j.Title.Contains(search));
        if (!string.IsNullOrEmpty(planet))
            query = query.Where(j => j.Planet.Contains(planet));
        if (minSalary.HasValue)
            query = query.Where(j => j.Salary >= minSalary);

        var jobs = await query.ToListAsync();

        List<Guid> userSkillIds = new();
        if (userId.HasValue)
        {
            userSkillIds = await _db.UserSkills
                .Where(us => us.UserId == userId.Value)
                .Select(us => us.SkillId)
                .ToListAsync();
        }

        return jobs.Select(j => new JobViewModel
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            Salary = j.Salary,
            Planet = j.Planet,
            EmployerId = j.EmployerId,
            Applications = j.Applications.Select(a => new ApplicationViewModel { }).ToList(),
            RequiredSkills = j.JobSkills.Select(js => js.Skill.Name).ToList(),
            CompatibilityPercent = userId.HasValue && j.JobSkills.Any()
                ? (int)Math.Round((double)j.JobSkills.Count(js => userSkillIds.Contains(js.SkillId)) / j.JobSkills.Count * 100)
                : null
        }).ToList();
    }

    public async Task<JobViewModel?> GetById(Guid id)
    {
        var job = await _jobRepository.Query()
            .Include(j => j.Employer)
            .Include(j => j.Applications)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (job == null) return null;

        return _mapper.Map<JobViewModel>(job);
    }

    public async Task Create(CreateJobViewModel model, Guid employerId)
    {
        var job = new Job
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Description = model.Description,
            Salary = model.Salary,
            Planet = model.Planet,
            EmployerId = employerId
        };

        if (model.SelectedSkillIds != null)
        {
            job.JobSkills = model.SelectedSkillIds.Select(skillId => new JobSkill
            {
                JobId = job.Id,
                SkillId = skillId
            }).ToList();
        }

        _db.Jobs.Add(job);
        await _db.SaveChangesAsync();
    }

    public async Task<List<JobViewModel>> GetByEmployer(Guid employerId)
    {
        var jobs = await _jobRepository.Query()
            .Where(j => j.EmployerId == employerId)
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
            .FirstOrDefaultAsync(j => j.Id == jobId && j.EmployerId == employerId);

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
