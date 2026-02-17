using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StarHire.Models.Domain;

namespace StarHire.Models.ViewModels.Applications
{
    public class CreateApplicationViewModel
    {
        public Guid JobId { get; set; }
        public Guid AlienId { get; set; }
        public string? Message { get; set; }
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    }
}
