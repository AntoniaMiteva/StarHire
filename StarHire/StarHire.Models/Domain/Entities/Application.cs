using Microsoft.AspNetCore.Identity;

namespace StarHire.Models.Domain.Entities
{
    public class Application
    {
        public Guid Id { get; set; }

        public Guid JobId { get; set; }

        public Job Job { get; set; }

        public Guid AlienId { get; set; }
        public IdentityUser Alien { get; set; }

        public ApplicationStatus Status { get; set; }
    }
}
