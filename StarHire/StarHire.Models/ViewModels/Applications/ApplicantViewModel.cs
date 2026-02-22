using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Models.ViewModels.Applications
{
    public class ApplicantViewModel
    {
        public Guid ApplicationId { get; set; }
        public Guid AlienId { get; set; }
        public string AlienEmail { get; set; } = string.Empty;
        public string AlienUserName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
