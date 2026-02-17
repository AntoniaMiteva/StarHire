using Microsoft.AspNetCore.Identity;

namespace StarHire.Models.Domain.Entities
{
    public class Application
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public virtual Job Job { get; set; }

        public Guid AlienId { get; set; }

        public IdentityUser<Guid> Alien { get; set; }

        public ApplicationStatus Status { get; set; }
        
    }
}
