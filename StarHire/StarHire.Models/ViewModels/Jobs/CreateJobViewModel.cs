using StarHire.Models.ViewModels.Skills;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarHire.Models.ViewModels.Jobs
{
    public class CreateJobViewModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be a positive number")]
        public decimal Salary { get; set; }

        [Required]
        public string Planet { get; set; } = string.Empty;
        public List<SkillCheckboxViewModel> Skills { get; set; } = new();
        public List<Guid> SelectedSkillIds { get; set; } = new();
    }
}
