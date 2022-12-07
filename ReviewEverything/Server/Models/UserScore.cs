using System.ComponentModel.DataAnnotations;

namespace ReviewEverything.Server.Models
{
    public class UserScore
    {
        [Range(1, 5)]
        public int Score { get; set; }
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
        public int CompositionId { get; set; }
        public Composition Composition { get; set; } = default!;
    }
}
