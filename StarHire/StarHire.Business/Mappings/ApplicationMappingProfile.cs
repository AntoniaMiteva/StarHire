using AutoMapper;
using StarHire.Models.Domain.Entities;
using StarHire.Models.ViewModels.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Business.Mappings
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Application, ApplicationViewModel>();
            //CreateMap <ApplicationCreateOrEditViewModel, Application>();
        }
    }
}
