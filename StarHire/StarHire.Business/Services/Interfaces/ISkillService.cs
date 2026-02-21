using StarHire.Models.ViewModels.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Business.Services.Interfaces
{
    public interface ISkillService
    {
        Task<ProfileSkillsViewModel> GetProfileSkillsAsync(Guid userId);
        Task UpdateProfileSkillsAsync(Guid userId, List<Guid> selectedSkillIds);
    }
}
