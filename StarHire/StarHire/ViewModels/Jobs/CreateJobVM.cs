using System.ComponentModel.DataAnnotations;

namespace StarHire.ViewModels.Jobs
{
    public class CreateJobVM
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Planet { get; set; }

        [Range(0, 1000000)]
        public decimal Salary { get; set; }
    }
}
