
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StarHire.Models.Domain.Entities
{
    public class Job
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }  = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public string Planet { get; set; } = string.Empty;

        public string EmployeerId { get; set; }
        public virtual IdentityUser Employeer { get; set; }

        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    }

}
