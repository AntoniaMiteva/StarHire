using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Models.Domain.Entities
{
    public class Favorite
    {
        public Guid Id { get; set; }
        public Guid JobId { get; set; }
        public virtual Job Job { get; set; }
        public Guid UserId { get; set; }
        public IdentityUser<Guid> User { get; set; }
    }
}
