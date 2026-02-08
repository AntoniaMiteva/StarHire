
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace StarHire.Models
{
    public class Job
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Salary { get; set; }

        public string Planet { get; set; }

        public string EmployerId { get; set; }

        public ICollection<Application> Applications { get; set; }
    }

}
