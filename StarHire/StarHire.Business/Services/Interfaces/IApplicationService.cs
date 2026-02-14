using StarHire.Models.ViewModels.Applications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Business.Services.Interfaces
{
    public interface IApplicationService
    {
        public Task ApplyAsync(Guid jobId, Guid userId, string message);
        public Task<List<ApplicationViewModel>> GetMyApplicationsAsync(Guid userId);


    }
}
