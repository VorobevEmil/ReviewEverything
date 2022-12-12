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
        public List<CloudImageRequest> CloudImages { get; set; } = new()
        {
            new CloudImageRequest()
            {
                Title = "Томас Шелби",
                Url = "https://sun9-75.userapi.com/impg/nxiRwgXrFZblfv3cYqyNGnxVhbYdKVfXxf9qqA/cqc_SqAXTPo.jpg?size=715x714&quality=96&sign=d8e5262b3af3cdcfa09a3faaee5e83d7&type=album"
            }
        };
    }
}
