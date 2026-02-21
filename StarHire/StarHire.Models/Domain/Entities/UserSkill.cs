using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Models.Domain.Entities
{
    public class UserSkill
    {
        public Guid UserId { get; set; }
        public IdentityUser<Guid> User { get; set; } = null!;

        public Guid SkillId { get; set; }
        public Skill Skill { get; set; } = null!;
    }
}
