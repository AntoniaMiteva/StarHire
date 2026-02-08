namespace StarHire.Models
{
    public class Application
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int JobId { get; set; }

        public Job Job { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
