namespace ReviewEverything.Shared.Contracts.Responses
{
    public class SimilarArticleReviewResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Subtitle { get; set; } = default!;
        public CloudImageResponse CloudImage { get; set; } = default!;
        public string Composition { get; set; } = default!;
        public string Author { get; set; } = default!;
        public int Comments { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
