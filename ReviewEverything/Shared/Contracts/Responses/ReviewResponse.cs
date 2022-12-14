namespace ReviewEverything.Shared.Contracts.Responses
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Subtitle { get; set; } = default!;
        public CloudImageResponse CloudImage { get; set; } = default!;
        public int AuthorScore { get; set; }
        public string AuthorId { get; set; } = default!;
        public string Author { get; set; } = default!;
        public int CompositionId { get; set; }
        public string Composition { get; set; } = default!;
        public int CategoryId { get; set; }
        public string Category { get; set; } = default!;
        public int CommentCount { get; set; } = default!;
        public int LikeUsers { get; set; } = default!;
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; } = default!;
    }
}