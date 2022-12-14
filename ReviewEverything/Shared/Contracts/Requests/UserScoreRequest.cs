using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Shared.Contracts.Requests
{
    public class UserScoreRequest
    {
        [Range(1, 5)]
        public int Score { get; set; }
        [Required]
        public int CompositionId { get; set; }
    }
}
