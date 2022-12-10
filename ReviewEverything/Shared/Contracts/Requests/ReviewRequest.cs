using System.Xml.Linq;

namespace ReviewEverything.Shared.Contracts.Requests
{
    public class ReviewRequest
    {
        public string Title { get; set; } = default!;
        public string Subtitle { get; set; } = default!;
        public string Body { get; set; } = default!;
        //public List<CloudImage> CloudImages { get; set; } = default!;
        public int AuthorScore { get; set; }
        public string AuthorId { get; set; } = default!;
        //public ApplicationUser Author { get; set; } = default!;
        public int CompositionId { get; set; }
        //public Composition Composition { get; set; } = default!;
        //public List<Tag> Tags { get; set; } = default!;
    }
}
