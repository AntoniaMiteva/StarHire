
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StarHire.Models.Domain.Entities
{
    public class Job
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Salary { get; set; }

        public string Planet { get; set; } = string.Empty;

        public Guid EmployerId { get; set; }

        public IdentityUser<Guid>? Employer { get; set; }

        public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

        public virtual ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
    }

}
