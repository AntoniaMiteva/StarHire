using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Models.Domain.Entities
{
    public class JobSkill
    {
        public Guid JobId { get; set; }
        public virtual Job Job { get; set; } = null!;
        public Guid SkillId { get; set; }
        public virtual Skill Skill { get; set; } = null!;
    }
}
