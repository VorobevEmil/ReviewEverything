namespace ReviewEverything.Server.Models
{
    public class UserScore
    {
        public int Score { get; set; }
        public string UserId { get; set; } = default!;
        public ApplicationUser User { get; set; } = default!;
        public int CompositionId { get; set; }
        public Composition Composition { get; set; } = default!;
    }
}
