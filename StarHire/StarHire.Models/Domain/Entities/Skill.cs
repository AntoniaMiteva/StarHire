using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Models.Domain.Entities
{
    public class Skill
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NormalizedName { get; set; } = string.Empty;

        public ICollection<UserSkill> UserSkills { get; set; } = new List<UserSkill>();
    }
}
