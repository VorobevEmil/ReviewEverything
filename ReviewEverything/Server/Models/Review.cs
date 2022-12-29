namespace ReviewEverything.Server.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Subtitle { get; set; } = default!;
        public string Body { get; set; } = default!;
        public List<CloudImage> CloudImages { get; set; } = default!;
        public int AuthorScore { get; set; }
        public string AuthorId { get; set; } = default!;
        public ApplicationUser Author { get; set; } = default!;
        public int CompositionId { get; set; }
        public Composition Composition { get; set; } = default!;
        public List<Comment> Comments { get; set; } = default!;
        public List<Tag> Tags { get; set; } = default!;
        public List<ApplicationUser> LikeUsers { get; set; } = default!;
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; } = default!;
    }
}
