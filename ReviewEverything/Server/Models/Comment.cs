namespace ReviewEverything.Server.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Body { get; set; } = default!;
        public int ReviewId { get; set; }
        public Review Review { get; set; } = default!; 
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
    }
}
