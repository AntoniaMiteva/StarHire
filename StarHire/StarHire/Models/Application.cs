namespace StarHire.Models
{
    public class Application
    {
        public int Id { get; set; }

        public int JobId { get; set; }

        public Job Job { get; set; }

        public string AlienId { get; set; }
        //public ApplicationUser Alien { get; set; }

        public ApplicationStatus Status { get; set; }
    }
}
