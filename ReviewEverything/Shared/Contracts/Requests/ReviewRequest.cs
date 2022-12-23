using System.ComponentModel.DataAnnotations;
using ReviewEverything.Shared.Contracts.Responses;

namespace ReviewEverything.Shared.Contracts.Requests
{
    public class ReviewRequest
    {
        [Required]
        public string Title { get; set; } = default!;
        [Required]
        public string Subtitle { get; set; } = default!;
        [Required]
        public string Body { get; set; } = default!;
        [Required]
        [Range(1, 10)]
        public int? AuthorScore { get; set; }
        [Required]
        public int? CompositionId { get; set; }

        public CompositionRequest? Composition { get; set; } = default!;
        public List<TagResponse> Tags { get; set; } = new();
        public List<CloudImageRequest> CloudImages { get; set; } = new();
    }
}
