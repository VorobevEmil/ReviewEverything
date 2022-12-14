namespace ReviewEverything.Shared.Contracts.Responses
{
    public class UserScoreResponse
    {
        public int Score { get; set; }
        public string UserId { get; set; } = default!;
        public int CompositionId { get; set; }
    }
}
