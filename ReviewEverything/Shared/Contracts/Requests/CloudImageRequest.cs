using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Contracts.Requests
{
    public class CloudImageRequest
    {
        [Required]
        public string Title { get; set; } = default!;
        [Required]
        public string Url { get; set; } = default!;
    }
}
