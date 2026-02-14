using Microsoft.AspNetCore.Identity;

namespace StarHire.Models.Domain.Entities
{
    public class Application
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public virtual Job Job { get; set; }

        public string AlienId { get; set; }
        public IdentityUser Alien { get; set; }

        public ApplicationStatus Status { get; set; }
    }
}
