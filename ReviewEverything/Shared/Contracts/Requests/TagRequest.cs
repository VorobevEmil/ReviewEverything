using ReviewEverything.Shared.Contracts.Responses;
using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Contracts.Requests
{
    public class TagRequest
    {
        [Required]
        public string Title { get; set; } = default!;
        public List<ReviewResponse> Reviews { get; set; } = default!;
    }
}
