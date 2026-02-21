using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Models.ViewModels.Skills
{
    public class ProfileSkillsViewModel
    {
        public List<SkillCheckboxViewModel> Skills { get; set; } = new();
    }
}
