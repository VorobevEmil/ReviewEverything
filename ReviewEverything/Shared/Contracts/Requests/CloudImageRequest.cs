using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Contracts.Requests
{
    public class CloudImageRequest
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = default!;
        [Required]
        public string Url { get; set; } = default!;
    }
}
