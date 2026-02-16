using StarHire.Models.ViewModels.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace StarHire.Business.Services.Interfaces
{
    public  interface IJobService
    {
        Task<List<JobViewModel>> GetAll(string? search, string? planet, decimal? minSalary);
        Task<JobViewModel?> GetById(Guid id);
        Task Create(CreateJobViewModel model, Guid employerId);
    }
}
