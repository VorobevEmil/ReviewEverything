namespace ReviewEverything.Server.Models
{
    public class Composition
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public List<Review> Reviews { get; set; } = default!;
        public List<UserScore> UserScores { get; set; } = default!;
    }
}
