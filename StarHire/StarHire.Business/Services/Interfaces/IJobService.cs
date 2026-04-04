using StarHire.Models.ViewModels.Applications;
using StarHire.Models.ViewModels.Jobs;
using StarHire.Models.ViewModels.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Business.Services.Interfaces
{
    public interface IJobService
    {
        Task<JobViewModel?> GetById(Guid id);
        Task Create(CreateJobViewModel model, Guid employerId);
        Task<List<JobViewModel>> GetByEmployer(Guid employerId);
        Task<List<ApplicantViewModel>> GetApplicants(Guid jobId, Guid employerId);
        Task<List<SkillCheckboxViewModel>> GetAllSkillsAsync();
        Task<List<JobViewModel>> GetAll(string? search, string? planet, decimal? minSalary, Guid? userId = null);
        Task<IEnumerable<JobViewModel>> GetRecommendedAsync(Guid userId);
    }
}
