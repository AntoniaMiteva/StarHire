using AutoMapper;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Business.Mappings
{
    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            
            CreateMap<CreateJobViewModel, Job>();

            
            CreateMap<Job, JobViewModel>();
        }
    }
}
