using Microsoft.AspNetCore.Identity;
using StarHire.Models.ViewModels.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Models.ViewModels.Applications
{
    public class ApplicationViewModel
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public JobViewModel Job { get; set; }

        public Guid AlienId { get; set; }
        public IdentityUser Alien { get; set; }

        public ApplicationStatus Status { get; set; }
    }
}
