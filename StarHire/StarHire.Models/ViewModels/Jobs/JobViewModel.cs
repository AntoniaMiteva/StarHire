using Microsoft.AspNetCore.Identity;
using StarHire.Models.ViewModels.Applications;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Models.ViewModels.Jobs
{
    public class JobViewModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public string Planet { get; set; } = string.Empty;

        public Guid EmployeerId { get; set; }
        public IdentityUser Employeer { get; set; }

        public ICollection<ApplicationViewModel> Applications { get; set; } = new List<ApplicationViewModel>();
    }
}
