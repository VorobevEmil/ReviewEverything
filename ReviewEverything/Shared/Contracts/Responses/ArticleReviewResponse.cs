using System.Xml.Linq;

namespace ReviewEverything.Shared.Contracts.Responses
{
    public class ArticleReviewResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Subtitle { get; set; } = default!;
        public string Body { get; set; } = default!;
        public List<CloudImageResponse> CloudImages { get; set; } = default!;
        public int AuthorScore { get; set; }
        public string AuthorId { get; set; } = default!;
        public string Author { get; set; } = default!;
        public int CompositionId { get; set; }
        public string Composition { get; set; } = default!;
        public int CategoryId { get; set; }
        public string Category { get; set; } = default!;
        public List<CommentResponse> Comments { get; set; } = default!;
        public List<TagResponse> Tags { get; set; } = default!;
        public List<string> LikeUsers { get; set; } = default!;
        public DateTime CreationDate { get; set; }
        public DateTime? UpdationDate { get; set; } = default!;
    }
}
