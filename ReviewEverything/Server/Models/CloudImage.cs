namespace ReviewEverything.Server.Models
{
    public class CloudImage
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Url { get; set; } = default!; 
        public int ReviewId { get; set; }
        public Review Review { get; set; } = default!;
    }
}
